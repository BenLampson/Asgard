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
        public AbsLoggerProvider? LoggerProvider { get; private set; }

        /// <summary>
        /// 本地缓存实例
        /// </summary>
        public AbsCache? CacheManager { get; private set; }

        /// <summary>
        /// 数据库管理器
        /// </summary>
        public AbsDataBaseManager? DBManager { get; private set; }

        /// <summary>
        /// 数据库管理器
        /// </summary>
        public AbsMQManager? MQ { get; private set; }


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
    }
}
