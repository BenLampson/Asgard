using System.Net;
using System.Text;

using Asgard.Core.ContextModules.Comm.Tcp;
using Asgard.Core.ContextModules.ConfigCenter.DBModels;
using Asgard.Core.ContextModules.ConfigCenter.TCPPackage;
using Asgard.Core.ContextModules.Logger.AbsInfos;

using static Asgard.Core.ContextModules.JsonConverterExtends.CommonSerializerOptions;

namespace Asgard.Core.ContextModules.ConfigCenter
{
    /// <summary>
    /// 系统配置客户端
    /// </summary>
    public class SystemConfigClient
    {
        private readonly TcpServer<byte[]> _server;
        private TcpClient<byte[]>? _client;
        private readonly IPEndPoint _endPoint;
        private readonly AbsLoggerProvider _loggerProvider;

        /// <summary>
        /// 当链接断开时
        /// </summary>
        public event Action? OnDisConnected;

        /// <summary>
        /// 当节点配置改变
        /// </summary>
        public event Action? NodeConfigChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="loggerProvider"></param>
        public SystemConfigClient(
            IPEndPoint endPoint,
            AbsLoggerProvider loggerProvider)
        {
            _endPoint = endPoint;
            _loggerProvider = loggerProvider;
            _server = new TcpServer<byte[]>(_endPoint,
               _loggerProvider, "SystemConfigClient",
               () => new ConfigCenterMessagePackage(_loggerProvider.CreateLogger<SystemConfigClient>()
               , Guid.NewGuid().ToByteArray()));
            var _logger = _loggerProvider.CreateLogger<SystemConfigClient>();
            _client = _server.GetClient("ConfigCenter");
            _client.GetPackage += _client_GetPackage;
            _client.OnDisConnected += _client_OnDisConnected;
        }

        private void _client_OnDisConnected(TcpClient<byte[]> obj)
        {
            _client?.Dispose();
            _client = _server.GetClient("ConfigCenter");
            _client.GetPackage += _client_GetPackage;
            _client.OnDisConnected += _client_OnDisConnected;
            OnDisConnected?.Invoke();
        }

        /// <summary>
        /// 连接至服务器
        /// </summary>
        public bool ConnectToServer()
        {
            if (_client is null)
            {
                return false;
            }
            return _client.Connect();
        }


        private void _client_GetPackage(AbsTcpPackage<byte[]> package)
        {
            if (package is ConfigCenterMessagePackage configCenterPackage
                  && configCenterPackage.TryGetOpType(out var type)
                  && type is not null
                  )
            {
                switch (type)
                {
                    case ConfigCenterOperationEnum.PointConfigChanged:
                        {
                            NodeConfigChanged?.Invoke();
                        }
                        break;
                    case ConfigCenterOperationEnum.SetMyPointName:
                    case ConfigCenterOperationEnum.ReloadData:
                    case ConfigCenterOperationEnum.GetByPointName:
                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// 获取某个节点的配置
        /// </summary>
        /// <param name="pointName">节点名称</param>
        /// <returns></returns>
        public SystemConfig? GetPointConfig(string pointName)
        {
            if (_client is null)
            {
                return default;
            }
            var json = Encoding.UTF8.GetBytes(pointName);
            using var package = _client.CreatePackage();
            byte[] data = new byte[4 + json.Length];
            BitConverter.GetBytes((int)ConfigCenterOperationEnum.GetByPointName).CopyTo(data.AsMemory());
            json.CopyTo(data.AsSpan()[4..]);
            _ = package.Write(data);
            if (package.TryRead(1000 * 300))
            {
                if (package.In is null || package.In.Length == 0)
                {
                    return default;
                }
                return System.Text.Json.JsonSerializer.Deserialize<SystemConfig>(package.In, CamelCaseChineseNameCaseInsensitive);
            }
            return default;
        }

        /// <summary>
        /// 让配置中心重新加载一次数据到缓存
        /// </summary>
        public void Reload()
        {
            if (_client is null)
            {
                return;
            }
            using var package = _client.CreatePackage();
            byte[] data = new byte[4];
            BitConverter.GetBytes((int)ConfigCenterOperationEnum.ReloadData).CopyTo(data.AsMemory());

            _ = package.Write(data);
        }

        /// <summary>
        /// 当节点配置发生改变通知
        /// </summary>
        /// <param name="pointName"></param>
        public void PointConfigChanged(string pointName)
        {
            if (_client is null)
            {
                return;
            }
            var json = Encoding.UTF8.GetBytes(pointName);
            using var package = _client.CreatePackage();
            byte[] data = new byte[4 + json.Length];
            BitConverter.GetBytes((int)ConfigCenterOperationEnum.PointConfigChanged).CopyTo(data.AsMemory());
            json.CopyTo(data.AsSpan()[4..]);
            _ = package.Write(data);
        }

        /// <summary>
        /// 设置当前节点名称
        /// </summary>
        /// <param name="pointName"></param>
        public void SetMyPointName(string pointName)
        {

            if (_client is null)
            {
                return;
            }
            var json = Encoding.UTF8.GetBytes(pointName);
            using var package = _client.CreatePackage();
            byte[] data = new byte[4 + json.Length];
            BitConverter.GetBytes((int)ConfigCenterOperationEnum.SetMyPointName).CopyTo(data.AsMemory());
            json.CopyTo(data.AsSpan()[4..]);
            _ = package.Write(data);
        }
    }
}
