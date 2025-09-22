using Asgard.Abstract.Cache;
using Asgard.Abstract.DataBase;
using Asgard.Abstract.Logger;
using Asgard.Abstract.Models.AsgardConfig;
using Asgard.Abstract.MQ;

namespace Asgard.Abstract
{
    /// <summary>
    /// 世界之树构建器
    /// </summary>
    public class YggdrasilBuilder
    {
        /// <summary>
        /// 事件ID,每次启动都会变
        /// </summary>
        public string EventID { get; private set; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 当前节点配置
        /// </summary>
        public NodeConfig? NodeConfig { get; private set; }

        /// <summary>
        /// 日志提供器
        /// </summary>
        public Func<NodeConfig, AbsLoggerProvider>? LoggerProvider { get; private set; }

        /// <summary>
        /// 本地缓存实例
        /// </summary>
        public Func<AbsLoggerProvider?, NodeConfig, AbsCache>? CacheManager { get; private set; }

        /// <summary>
        /// 数据库管理器
        /// </summary>
        public Func<AbsLoggerProvider?, NodeConfig, AbsDataBaseManager>? DBManager { get; private set; }

        /// <summary>
        /// 数据库管理器
        /// </summary>
        public AbsMQManager? MQ { get; private set; }

        /// <summary>
        /// 设置缓存提供器
        /// </summary>
        /// <param name="cacheLoader"></param>
        /// <returns></returns>
        public YggdrasilBuilder SetCacheManager(Func<AbsLoggerProvider?, NodeConfig, AbsCache> cacheLoader)
        {
            this.CacheManager = cacheLoader;
            return this;
        }

        /// <summary>
        /// 设置节点配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public YggdrasilBuilder SetNodeConfig(NodeConfig config)
        {
            this.NodeConfig = config;
            return this;
        }

        /// <summary>
        /// 设置数据库管理器
        /// </summary>
        /// <param name="dbManagerLoader"></param>
        /// <returns></returns>
        public YggdrasilBuilder SetDBManager(Func<AbsLoggerProvider?, NodeConfig, AbsDataBaseManager> dbManagerLoader)
        {
            this.DBManager = dbManagerLoader;
            return this;
        }

        /// <summary>
        /// 设置日志的提供器
        /// </summary>
        /// <param name="providerLoader"></param>
        /// <returns></returns>
        public YggdrasilBuilder SetLoggerProvider(Func<NodeConfig, AbsLoggerProvider> providerLoader)
        {
            this.LoggerProvider = providerLoader;
            return this;
        }
    }
}
