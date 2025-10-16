using Asgard.Abstract.Models.AsgardConfig.ChildConfigModel;
using Asgard.Abstract.Models.AsgardConfig.ChildConfigModel.LogConfig;
using Asgard.Abstract.Models.AsgardConfig.ChildConfigModel.PluginConfig;

namespace Asgard.Abstract.Models.AsgardConfig
{
    /// <summary>
    /// 节点配置
    /// </summary>
    public class NodeConfig
    {
        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string Name { get; set; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 对这个服务的描述信息
        /// </summary>
        public string Desc { get; set; } = string.Empty;

        /// <summary>
        /// 只启动Job服务
        /// </summary>
        public bool JustJobServer { get; set; }

        /// <summary>
        /// 自己就是插件
        /// </summary>
        public bool SelfAsAPlugin { get; set; }
        /// <summary>
        /// 如果自己是插件,描述一下
        /// </summary>
        public PluginItem SelfPluginInfo { get; set; } = new();

        /// <summary>
        /// 插件文件夹,默认Plugins
        /// </summary>
        public string PluginsFolder { get; set; } = "Plugins";

        /// <summary>
        /// 配置中心的配置,如果selfhost是false,那么其他配置将会被覆盖
        /// </summary>
        public ConfigCenterConfig ConfigCenter { get; set; } = new();

        /// <summary>
        /// ID生成器配置
        /// </summary>
        public IDWorker IdWorker { get; set; } = new();

        /// <summary>
        /// Redis配置
        /// </summary>
        public RedisConfig RedisConfig { get; set; } = new();

        /// <summary>
        /// 默认数据库
        /// </summary>
        public DefaultDBConfig DefaultDB { get; set; } = new();


        /// <summary>
        /// MQ配置
        /// </summary>
        public MQConfig MqConfig { get; set; } = new();

        /// <summary>
        /// 所有的插件
        /// </summary>
        public PluginItem[] Plugins { get; set; } = Array.Empty<PluginItem>();
        /// <summary>
        /// 认证配置
        /// </summary>
        public AuthConfig AuthConfig { get; set; } = new();

        /// <summary>
        /// 系统日志配置
        /// </summary>
        public LogOptions SystemLog { get; set; } = new LogOptions();

        /// <summary>
        /// webAPI配置信息,如果jobserver则可能为空,如果为空,则一定为job server
        /// </summary>
        public WebApiConfig WebAPIConfig { get; set; } = new();

        /// <summary>
        /// 用户自定义的设置,可以配置这个节点的特殊信息
        /// </summary>
        public Dictionary<string, string> CustomSettings { get; set; } = new();


    }
}
