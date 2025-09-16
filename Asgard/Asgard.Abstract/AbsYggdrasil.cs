using Asgard.Abstract.Cache;
using Asgard.Abstract.Logger;
using Asgard.Abstract.Models.AsgardConfig;

namespace Asgard.Abstract
{
    /// <summary>
    /// 抽象级别的世界之树
    /// </summary>
    public abstract class AbsYggdrasil
    {
        /// <summary>
        /// 事件ID,每次启动都会变
        /// </summary>
        public string EventID { get; set; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 当前节点配置
        /// </summary>
        public NodeConfig? NodeConfig { get; private set; }

        /// <summary>
        /// 日志提供器
        /// </summary>
        public AbsLoggerProvider? LoggerProvider { get; private set; }

        /// <summary>
        /// 本地缓存实例
        /// </summary>
        public AbsCache? CacheManager { get; set; }


        /// <summary>
        /// 设置缓存管理器
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        public AbsYggdrasil SetCacheManager(AbsCache cache)
        {
            this.CacheManager = cache;
            return this;
        }

        /// <summary>
        /// 设置日志提供器
        /// </summary>
        /// <param name="loggerProvider"></param>
        /// <returns></returns>
        public AbsYggdrasil SetLoggerProvider(AbsLoggerProvider loggerProvider)
        {
            this.LoggerProvider = loggerProvider;
            return this;
        }

        /// <summary>
        /// 设置节点配置
        /// </summary>
        /// <param name="nodeConfig">对应配置</param>
        /// <returns></returns>
        public AbsYggdrasil SetNodeConfig(NodeConfig nodeConfig)
        {
            this.NodeConfig = nodeConfig;
            return this;
        }
    }
}
