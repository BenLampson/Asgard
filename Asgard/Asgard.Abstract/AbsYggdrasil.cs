using Asgard.Abstract.Auth;
using Asgard.Abstract.Cache;
using Asgard.Abstract.DataBase;
using Asgard.Abstract.Job;
using Asgard.Abstract.Logger;
using Asgard.Abstract.Models.AsgardConfig;
using Asgard.Abstract.MQ;

namespace Asgard.Abstract
{
    /// <summary>
    /// 抽象级别的世界之树
    /// </summary>
    public abstract class AbsYggdrasil
    {
        /// <summary>
        /// 插件加载管理器
        /// </summary>
        public PluginLoaderManager? PluginManager { get; set; }
        /// <summary>
        /// 事件ID,每次启动都会变
        /// </summary>
        public string EventID { get; init; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 当前节点配置
        /// </summary>
        public NodeConfig? NodeConfig { get; init; }
        /// <summary>
        /// 权限模块 
        /// </summary>
        public AbsAuthManager? AuthManager { get; init; }

        /// <summary>
        /// 日志提供器
        /// </summary>
        public AbsLoggerProvider? LoggerProvider { get; init; }

        /// <summary>
        /// 数据库管理器
        /// </summary>
        public AbsDataBaseManager? DBManager { get; init; }

        /// <summary>
        /// Job管理器
        /// </summary>
        public AbsJobManager? JobManager { get; init; }

        /// <summary>
        /// 本地缓存实例
        /// </summary>
        public AbsCache? CacheManager { get; init; }

        /// <summary>
        /// MQ库管理器
        /// </summary>
        public AbsMQManager? MQ { get; init; }



        /// <summary>
        /// 获取一个新的上下文对象
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public AsgardContext GetContext()
        {
            if (NodeConfig == null)
            {
                throw new ArgumentNullException("请先初始化系统配置");
            }
            var context = new AsgardContext(NodeConfig, LoggerProvider, CacheManager, DBManager, MQ, AuthManager, Guid.NewGuid().ToString("N"));
            return context;
        }

    }
}
