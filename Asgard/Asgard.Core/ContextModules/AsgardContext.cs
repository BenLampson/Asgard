using System.Runtime.CompilerServices;

using Asgard.Core.ContextModules.AspNet.Auth;
using Asgard.Core.ContextModules.Cache;
using Asgard.Core.ContextModules.ConfigCenter.DBModels;
using Asgard.Core.ContextModules.DB;
using Asgard.Core.ContextModules.Logger;
using Asgard.Core.ContextModules.Logger.AbsInfos;
using Asgard.Core.ContextModules.Logger.Models;
using Asgard.Core.ContextModules.MQ;
using Asgard.Core.ContextModules.PluginLoader;

namespace Asgard.Core.ContextModules
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
        public AuthManager? Auth { get; init; }

        /// <summary>
        /// 数据库模块
        /// </summary>
        public DataBaseManager DB { get; init; }
        /// <summary>
        /// 日志提供器
        /// </summary>
        public AbsLoggerProvider LoggerProvider { get; init; }
        /// <summary>
        /// 当前容器插件
        /// 如果是配置中心服务自己,则没有
        /// </summary>
        public PluginLoaderManager? Plugins { get; init; }

        /// <summary>
        /// 创建一个新的上下文
        /// </summary>
        public Func<AsgardContext>? CreateNewContext { get; init; }

        /// <summary>
        /// MQ服务
        /// </summary>
        public AbsMQManager? MQ { get; init; }
        /// <summary>
        /// 缓存实例对象
        /// </summary>
        public CacheManager Cache { get; init; }
        /// <summary>
        /// 当前节点配置
        /// 如果是配置中心服务自己,则没有
        /// </summary>
        public SystemConfig NodeConfig { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cache"></param> 
        /// <param name="loggerProvider"></param>
        /// <param name="db"></param>
        /// <param name="plugin"></param> 
        /// <param name="nodeConfig"></param>
        /// <param name="auth"></param>
        /// <param name="eventID"></param> 
        /// <param name="createNewContext"></param> 
        public AsgardContext(
            CacheManager cache,
            AbsLoggerProvider loggerProvider,
            DataBaseManager db,
            PluginLoaderManager? plugin,
            AbsMQManager? mq,
            AuthManager? auth,
            SystemConfig nodeConfig,
            string? eventID,
            Func<AsgardContext>? createNewContext
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
            Plugins = plugin;
            CreateNewContext = createNewContext;
        }
        /// <summary>
        /// 创建一个日志处理器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enterAndExitLevel"></param>
        /// <param name="filePath"></param>
        /// <param name="num"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public LogHandler CreateLogHandler<T>(LogLevelEnum enterAndExitLevel, [CallerFilePath] string filePath = "", [CallerLineNumber] int num = 0, [CallerMemberName] string name = "")
        {
            return new LogHandler(LoggerProvider.CreateLogger<T>(), enterAndExitLevel, EventID, filePath, num, name);
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
