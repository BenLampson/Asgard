using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;

using Asgard.Abstract.MQ;
using Asgard.ConfigCenter;
using Asgard.ConfigCenter.DBModels;
using Asgard.DataBaseManager.FreeSql;
using Asgard.Extends.Json;
using Asgard.Logger.FreeSqlProvider;

namespace Asgard.Hosts.AspNetCore
{
    /// <summary>
    /// 一个内阁系统的管理器,你可以利用该系统快速启动项目
    /// 注意,为了保持系统的一致性,就算是配置中心自己,本质也是自己启动了自己
    /// </summary>
    public partial class HostManager
    {
        /// <summary>
        /// 当前启动事件ID
        /// </summary>
        private string EventID { get; init; } = Guid.NewGuid().ToString("N");


        /// <summary>
        /// 配置中心客户端
        /// </summary>
        public SystemConfigClient? ConfigClient { get; private set; }

        /// <summary>
        /// 当前节点配置
        /// </summary>
        public SystemConfig NodeConfig { get; private set; }

        /// <summary>
        /// 日志提供器
        /// </summary>
        public LoggerProvider? LoggerProvider { get; private set; }

        /// <summary>
        /// 数据库实例
        /// </summary>
        public FreeSqlManager? DB { get; private set; }

        /// <summary>
        /// 认证管理器
        /// </summary>
        public AuthManager.AspNetCore.AuthManager? AuthManager { get; private set; }

        /// <summary>
        /// 本地缓存实例
        /// </summary>
        public CacheManager? CacheManager { get; private set; }

        /// <summary>
        /// job管理器
        /// </summary>
        public JobManager? JobManager { get; private set; }

        /// <summary>
        /// MQ服务
        /// </summary>
        public AbsMQManager? MQ { get; private set; }

        /// <summary>
        /// 插件管理器
        /// </summary>
        public PluginLoaderManager? PluginManager { get; private set; }


        /// <summary>
        /// Asp.net app
        /// </summary>
        public WebApplication? WebApp { get; private set; }

        /// <summary>
        /// 利用类似main函数的参数启动项目
        /// </summary>
        /// <param name="systemArgs"></param>
        public HostManager(string[] systemArgs)
        {
            var pointName = systemArgs.FirstOrDefault(item => item.StartsWith("pn:"));
            var configServerAddress = systemArgs.FirstOrDefault(item => item.StartsWith("csa:"));
            var csp = systemArgs.FirstOrDefault(item => item.StartsWith("csp:"));
            if (string.IsNullOrWhiteSpace(pointName)
                ||
                string.IsNullOrWhiteSpace(configServerAddress)
                ||
                string.IsNullOrWhiteSpace(csp)
                || !ushort.TryParse(csp.Replace("csp:", ""), out var port)
                )
            {
                throw new ArgumentException("启动参数不足,参数应当为:pn:配置中心对应节点名称 cas:配置中心ip地址 csp:配置中心端口号");
            }
            NodeConfig = new();
            NodeConfig.Name = pointName.Replace("pn:", "");
            NodeConfig.Value.ConfigCenter.ConfigCenter = configServerAddress.Replace("csa:", "");
            NodeConfig.Value.ConfigCenter.ConfigCenterPort = port;

        }

        /// <summary>
        /// 普通的利用读取配置文件启动项目,配置文件必须在系统应用程序目录下,叫做appsettings.json
        /// 如果不存在,会创建一个默认的出来
        /// </summary>
        public HostManager()
        {
            if (!File.Exists(Path.Combine(AppContext.BaseDirectory, "appsettings.json")))
            {
                File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), JsonSerializer.Serialize(new SystemConfig()));
                throw new ArgumentException($"配置文件不存在,系统现在将在程序目录下生成一个范本,请填写!");
            }
            var configStr = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
            var tmpConfig = JsonSerializer.Deserialize<SystemConfig>(configStr, CommonSerializerOptions.CamelCaseChineseNameCaseInsensitive);
            if (tmpConfig is null || string.IsNullOrWhiteSpace(tmpConfig.Name))
            {
                throw new ArgumentException($"点位名称为空!");
            }
            if (string.IsNullOrWhiteSpace(tmpConfig.Value.ConfigCenter.ConfigCenter)
                || !IPAddress.TryParse(tmpConfig.Value.ConfigCenter.ConfigCenter, out var _)
                )
            {
                throw new ArgumentException($"配置中心的地址错误!当前我获取到的是:{tmpConfig.Value.ConfigCenter.ConfigCenter ?? ""}");
            }
            NodeConfig = tmpConfig;
        }

        /// <summary>
        /// 启动配置中心服务,你可以无脑调用该函数,如果不是自宿主,该函数不会有任何操作
        /// </summary>
        public HostManager Start()
        {
            StartConfigCenterPartial();//启动配置中心
            UseConfigStartSystem();//根据配置启动服务

            Console.WriteLine("开始加载插件");
            LoadPluginFromConfig();
            LoadPluginFromFolder();
            LoadSelfAsAPlugin();
            Console.WriteLine("所有插件加载完毕");

            return this;

        }

        /// <summary>
        /// 启动宿主,到了这里,那就要进行自宿主了
        /// </summary>
        public async void StartHost()
        {
            InitAspNet();//初始化asp.net部分


            if (WebApp is not null)//web宿主
            {
                _ = WebApp.Lifetime.ApplicationStarted.Register(() =>
                {
                    try
                    {
                        SystemStart();
                    }
                    catch (Exception ex)
                    {
                        LoggerProvider?.CreateLogger<HostManager>().Critical("系统启动失败!", exception: ex, eventID: EventID);
                        _ = Task.Run(async () => { await WebApp.StopAsync(); });
                    }
                });
                _ = WebApp.Lifetime.ApplicationStopping.Register(() =>
                {
                    try
                    {
                        NotifyStoping();
                    }
                    catch (Exception ex)
                    {
                        LoggerProvider?.CreateLogger<HostManager>().Critical("系统停止失败!", exception: ex, eventID: EventID);
                    }
                });
                LoggerProvider?.CreateLogger<HostManager>().Critical("系统启动.", eventID: EventID);
                WebApp.Start();
            }

            if (WebApp is not null)
            {
                WebApp.WaitForShutdown();
            }
            else
            {
                using var waitForProcessShutdownStart = new ManualResetEventSlim();
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    _ = PosixSignalRegistration.Create((PosixSignal)15, (context) =>
                    {
                        context.Cancel = true;
                        waitForProcessShutdownStart.Set();
                    });
                }
                else
                {

                    _ = PosixSignalRegistration.Create(PosixSignal.SIGINT, (context) =>
                    {
                        context.Cancel = true;
                        waitForProcessShutdownStart.Set();
                    });
                }

                waitForProcessShutdownStart.Wait();
            }


            if (WebApp is not null)
            {
                await WebApp.StopAsync();
            }
            else
            {
                NotifyStoping();
            }
            Console.WriteLine("系统关闭完成,尝试销毁资源.");
            DB?.Default?.Dispose();
            Console.WriteLine("销毁完成.");
            Console.WriteLine("系统已停止.");
            LoggerProvider?.CreateLogger<HostManager>().Critical("系统已停止.", eventID: EventID);

        }

        /// <summary>
        /// 创建一个上下文,如果不适用宿主,用这个即可
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ShangShuShengContext CreateContext()
        {
            if (
                CacheManager is null ||
                NodeConfig is null ||
                LoggerProvider is null ||
                DB is null
                )
            {
                throw new Exception($"系统环境还没有初始化.缓存管理器:{CacheManager is not null} " +
                    $"节点配置:{NodeConfig is null} " +
                    $"数据库配置:{DB is null} " +
                    $"日志管理器:{LoggerProvider is null}");
            }
            return new ShangShuShengContext(
                    CacheManager,
                    LoggerProvider,
                    DB,
                    PluginManager,
                    MQ,
                    AuthManager,
                    NodeConfig,
                    Guid.NewGuid().ToString("N"),
                    CreateContext);
        }
    }
}
