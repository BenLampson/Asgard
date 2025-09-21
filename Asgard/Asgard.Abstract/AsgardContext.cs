using Asgard.Abstract.Auth;
using Asgard.Abstract.Cache;
using Asgard.Abstract.DataBase;
using Asgard.Abstract.Logger;
using Asgard.Abstract.Models.AsgardConfig;
using Asgard.Abstract.MQ;

namespace Asgard.Abstract
{
    /// <summary>
    /// 阿斯加德上下文对象
    /// </summary>
    public sealed class AsgardContext : IDisposable
    {
        /// <summary>
        /// 事件ID,每次请求/事件都会发生变化
        /// </summary>
        public string EventID { get; set; } = Guid.NewGuid().ToString("N");


        /// <summary>
        /// 权限模块
        /// 如果是配置中心服务自己,则没有
        /// </summary>
        public AbsAuthManager? Auth { get; init; }

        /// <summary>
        /// 数据库模块
        /// </summary>
        public AbsDataBaseManager DB { get; init; }
        /// <summary>
        /// 日志提供器
        /// </summary>
        public AbsLoggerProvider? LoggerProvider { get; init; }


        /// <summary>
        /// MQ服务
        /// </summary>
        public AbsMQManager? MQ { get; init; }
        /// <summary>
        /// 缓存实例对象
        /// </summary>
        public AbsCache? Cache { get; init; }
        /// <summary>
        /// 当前节点配置
        /// 如果是配置中心服务自己,则没有
        /// </summary>
        public NodeConfig NodeConfig { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cache"></param> 
        /// <param name="loggerProvider"></param>
        /// <param name="db"></param>
        /// <param name="mq"></param> 
        /// <param name="nodeConfig"></param>
        /// <param name="auth"></param>
        /// <param name="eventID"></param>   
        public AsgardContext(
            NodeConfig nodeConfig,
            AbsLoggerProvider? loggerProvider,
            AbsCache? cache,
            AbsDataBaseManager db,
            AbsMQManager? mq,
            AbsAuthManager? auth,
            string? eventID
            )
        {
            if (!string.IsNullOrWhiteSpace(eventID))
            {
                EventID = eventID;
            }
            NodeConfig = nodeConfig;
            Cache = cache;
            LoggerProvider = loggerProvider;
            DB = db;
            MQ = mq;
            Auth = auth;
        }



        #region 析构部分

        /// <summary>
        /// 是否已经销毁
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// 析构函数
        /// </summary>
        ~AsgardContext()
        {
            Dispose(false);
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose(bool _)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
