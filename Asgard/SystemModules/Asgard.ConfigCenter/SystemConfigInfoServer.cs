using System.Collections.Concurrent;
using System.Net;
using System.Text;

using Asgard.Abstract;
using Asgard.Abstract.Communication.Tcp;
using Asgard.Abstract.Logger;
using Asgard.Abstract.Models.AsgardConfig;
using Asgard.Abstract.Models.AsgardConfig.ChildConfigModel;
using Asgard.Abstract.Models.AsgardConfig.ChildConfigModel.LogConfig;
using Asgard.ConfigCenter.DBModels;
using Asgard.ConfigCenter.TCPPackage;
using Asgard.Tools;

using static Asgard.Extends.Json.CommonSerializerOptions;
namespace Asgard.ConfigCenter
{
    /// <summary>
    /// 系统配置服务
    /// </summary>
    public class SystemConfigInfoServer
    {
        private readonly TcpServer<byte[]> _server;
        private readonly AsgardContext<IFreeSql> _context;
        private readonly ConcurrentDictionary<string, List<TcpClient<byte[]>>> _pointNameClientIDMapping = new();
        private readonly ConcurrentDictionary<string, string> _clientIDPointNameMapping = new();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="loggerProvider"></param>
        /// <param name="context"></param>
        /// <param name="config"></param>
        public SystemConfigInfoServer(
            IPEndPoint endPoint,
            AbsLoggerProvider loggerProvider,
            AsgardContext<IFreeSql> context,
            SystemConfig config)
        {
            var _logger = loggerProvider.CreateLogger<SystemConfigInfoServer>();
            _server = new TcpServer<byte[]>(endPoint,
                loggerProvider, "SystemConfigInfoServer",
                () => new ConfigCenterMessagePackage(_logger, Guid.NewGuid().ToByteArray()));
            _context = context;
            if (!context.DB.Default.DbFirst.ExistsTable("asgard_system_configs"))
            {
                InitConfigCenterServer(context, config);
            }
            SystemConfigInfo.RefreshCache(context);
        }

        /// <summary>
        /// 初始化配置中心服务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="config"></param>
        private void InitConfigCenterServer(AsgardContext<IFreeSql> context, SystemConfig config)
        {
            context.DB.Default.CodeFirst.SyncStructure(typeof(SystemConfig));

            var (key, iv) = AuthKVToolsMethod.CreateNewAesKeyAndVi();
            var defaultNodeConfig = new NodeConfig
            {
                Desc = "系统初始化默认配置",
                IdWorker = new IDWorker(),
                JustJobServer = false,
                PluginsFolder = "Plugins",
                DefaultDB = config.Value.DefaultDB,
                ConfigCenter = new(),
                CustomeSettings = new(),
                MqConfig = new(),
                Plugins = [],
                RedisConfig = new(),
                SystemLog = new LogOptions() { EnableConsole = true, MinLevel = LogLevelEnum.Trace },
                WebAPIConfig = new WebApiConfig()
                {
                    ApiPrefix = "",
                    CertificateFile = "./ShangShuSheng.pfx",
                    CertificatePassword = "!QAZ2wsx",
                    HttpPort = 11080,
                    HttpsPort = 11443,
                    HttpV2Port = 11081,
                    SwaggerUrlPrefix = "",
                    UseSwagger = true,
                    WithOrigins = new string[] { "*" }
                },
                AuthConfig = new AuthConfig()
                {
                    Audience = "AllUser",
                    Issuer = "ShangShuSheng",
                    JwtKey = AuthKVToolsMethod.CreateNewHMACSHA256Key(),
                    AesKey = key,
                    AesIV = iv,
                },
            };

            var initConfig = new SystemConfig
            {
                ID = 1,
                Name = "DefaultPoint",
                Value = defaultNodeConfig

            };
            _ = context.DB.Default.Insert(new SystemConfig[] {
                    initConfig}).ExecuteAffrows();
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            _server.GotClient += (client) =>
            {
                client.OnDisConnected += (clientInstance) =>
                {
                    _ = _clientIDPointNameMapping.Remove(clientInstance.ID, out var pointName);
                    _ = _pointNameClientIDMapping.AddOrUpdate(pointName ?? "", new List<TcpClient<byte[]>>(), (pointName, oldValue) =>
                     {
                         oldValue.RemoveAt(oldValue.FindIndex(item => item.ID == clientInstance.ID));
                         return oldValue;
                     });
                };
                client.GetPackage += (package) =>
                {
                    if (package is ConfigCenterMessagePackage configCenterPackage
                    && configCenterPackage.TryGetOpType(out var type)
                    && type is not null
                    )
                    {
                        switch (type)
                        {
                            case ConfigCenterOperationEnum.GetByPointName:
                                {
                                    if (!configCenterPackage.TryGetStringBody(out var pointName) || pointName is null)
                                    {
                                        _ = package.Write(Array.Empty<byte>());
                                        return;
                                    }
                                    var config = SystemConfigInfo.GetByNodeName(_context, pointName);
                                    if (config is null)
                                    {
                                        _ = package.Write(Array.Empty<byte>());
                                        return;
                                    }

                                    var jsonStr = System.Text.Json.JsonSerializer.Serialize(config, CamelCaseChineseNameCaseInsensitive);
                                    if (string.IsNullOrWhiteSpace(jsonStr))
                                    {
                                        _ = package.Write(Array.Empty<byte>());
                                        return;
                                    }
                                    _ = package.Write(Encoding.UTF8.GetBytes(jsonStr));
                                }
                                break;
                            case ConfigCenterOperationEnum.ReloadData:
                                SystemConfigInfo.RefreshCache(_context);
                                break;
                            case ConfigCenterOperationEnum.SetMyPointName:
                                {
                                    if (!configCenterPackage.TryGetStringBody(out var pointName) || pointName is null)
                                    {
                                        return;
                                    }
                                    _ = _clientIDPointNameMapping.AddOrUpdate(client.ID, pointName, (clientID, oldValue) => pointName);
                                    _ = _pointNameClientIDMapping.AddOrUpdate(pointName ?? "", new List<TcpClient<byte[]>>() { client }, (pointName, oldValue) =>
                                    {
                                        if (!oldValue.Exists(item => item.Equals(client.ID)))
                                        {
                                            oldValue.Add(client);
                                        }
                                        return oldValue;
                                    });
                                }
                                break;
                            case ConfigCenterOperationEnum.PointConfigChanged:
                                {
                                    if (!configCenterPackage.TryGetStringBody(out var pointName) || pointName is null)
                                    {
                                        return;
                                    }
                                    CastboardPointConfigChange(pointName);
                                }
                                break;
                            default:
                                break;
                        }

                    }

                };
            };
            _server.Start();
        }

        /// <summary>
        /// 给当配置中心点内的所有关系人发送节点配置改动消息
        /// </summary>
        /// <param name="pointName"></param>
        public void CastboardPointConfigChange(string pointName)
        {
            if (_pointNameClientIDMapping.TryGetValue(pointName, out var clients))
            {
                clients.ForEach(item =>
                {
                    try
                    {
                        var package = item.CreatePackage();
                        byte[] data = new byte[4];
                        BitConverter.GetBytes((int)ConfigCenterOperationEnum.PointConfigChanged).CopyTo(data.AsMemory());
                        _ = package.Write(data);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                });
            }
        }
    }
}
