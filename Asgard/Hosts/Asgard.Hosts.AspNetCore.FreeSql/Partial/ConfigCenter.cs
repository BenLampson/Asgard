using System.Net;

namespace Asgard.Hosts.AspNetCore.FreeSql
{
    /// <summary>
    /// 一个内阁系统的管理器,你可以利用该系统快速启动项目
    /// 注意,为了保持系统的一致性,就算是配置中心自己,本质也是自己启动了自己
    /// </summary>
    public partial class HostManager
    {
        /// <summary>
        /// 启动配置中心服务,你可以无脑调用该函数,如果不是自宿主,该函数不会有任何操作
        /// </summary>
        public void StartConfigCenterPartial()
        {
            var consoleLoggerProvider = Asgard.Logger.FreeSqlProvider.LoggerProvider.CreateConsoleLogProvider(LogLevelEnum.Trace);
            //如果不是自宿主,直接走
            if (NodeConfig.Value.ConfigCenter.SelfHostConfigCenter)
            {

                var configCenterDbManager = new FreeSqlManager(consoleLoggerProvider
                    , NodeConfig.Value);
                var configCenterCacheManager = new CacheManager(consoleLoggerProvider);
                configCenterCacheManager.PushRedis(NodeConfig.Value.RedisConfig.GetConnStr(), consoleLoggerProvider.CreateLogger<CacheManager>());

                var configCenterContext = new AsgardContext(
                      nodeConfig: NodeConfig.Value
                     , loggerProvider: consoleLoggerProvider
                     , cache: configCenterCacheManager
                     , db: configCenterDbManager
                     , mq: null
                     , auth: null
                     , eventID: Guid.NewGuid().ToString("N")
                );

                //var server = new SystemConfigInfoServer(new IPEndPoint(IPAddress.Any, NodeConfig.Value.ConfigCenter.ConfigCenterPort),
                //    consoleLoggerProvider, configCenterContext, NodeConfig);
                //server.Start();
            }
            else if (NodeConfig.Value.ConfigCenter.WithOutConfigCenter)
            {
                consoleLoggerProvider.CreateLogger<HostManager>().Warning("配置了非配置中心模式,将会以自我查找的方式启动!");
            }
            else
            {
                ConfigClient = new SystemConfigClient(IPEndPoint.Parse($"{NodeConfig.Value.ConfigCenter.ConfigCenter}:{NodeConfig.Value.ConfigCenter.ConfigCenterPort}"), consoleLoggerProvider);
                ConfigClient.OnDisConnected += _configClient_OnDisConnected;
                ConfigClient.NodeConfigChanged += _configClient_NodeConfigChanged;
                ConnToConfigCenterServer();
                ConfigClient.SetMyPointName(NodeConfig.Name);

                if (ConfigClient is null)
                {
                    throw new Exception("系统启动失败!还没有初始化配置中心!");
                }
                var pointConfig = ConfigClient.GetPointConfig(NodeConfig.Name);
                if (pointConfig is null)
                {
                    throw new Exception($"无法获取节点:{NodeConfig.Name}的配置信息");
                }
                NodeConfig = pointConfig;
            }

        }
        private void _configClient_NodeConfigChanged()
        {
            if (ConfigClient is null || NodeConfig is null)
            {
                return;
            }
            var pointConfig = ConfigClient.GetPointConfig(NodeConfig.Name);
            if (pointConfig is null)
            {
                return;
            }
            NodeConfig = pointConfig;
        }
        /// <summary>
        /// 自动重连
        /// </summary>
        private void _configClient_OnDisConnected()
        {
            ConnToConfigCenterServer();
        }
        /// <summary>
        /// 连接至配置中心
        /// </summary>
        private void ConnToConfigCenterServer()
        {
            if (ConfigClient is null)
            {
                return;
            }
            while (true)
            {
                try
                {
                    _ = ConfigClient.ConnectToServer();
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"配置中心链接错误:{ex.Message}, 1秒后重试.");
                    Task.Delay(1000 * 1).Wait();
                    continue;
                }
            }
            Console.WriteLine($"配置中心链接成功!");
        }

    }
}
