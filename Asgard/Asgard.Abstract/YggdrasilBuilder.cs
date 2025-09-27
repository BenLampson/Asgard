using Asgard.Abstract.Auth;
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
        /// 构造函数,可以传入节点配置
        /// </summary>
        /// <param name="config"></param>
        public YggdrasilBuilder(NodeConfig config)
        {
            this.NodeConfig = config;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public YggdrasilBuilder()
        {

        }
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
        /// 权限模块 
        /// </summary>
        public Func<AbsLoggerProvider?, NodeConfig, AbsAuthManager>? AuthProvider { get; init; }

        /// <summary>
        /// 本地缓存实例
        /// </summary>
        public Func<AbsLoggerProvider?, NodeConfig, AbsCache>? CacheManager { get; private set; }

        /// <summary>
        /// 数据库管理器
        /// </summary>
        public Func<AbsLoggerProvider?, NodeConfig, AbsDataBaseManager>? DBManager { get; private set; }

        /// <summary>
        /// MQ库管理器
        /// </summary>
        public AbsMQManager? MQ { get; private set; }

        /// <summary>
        /// 事件ID生成器
        /// </summary>
        public Func<string> EventIDBuilder { get; init; } = () => Guid.NewGuid().ToString("N");

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
