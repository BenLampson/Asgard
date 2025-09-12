using Asgard.Core.ContextModules.DB;
using Asgard.Core.ContextModules.Logger.AbsInfos;

namespace Asgard.Core.ContextModules.PluginLoader
{
    /// <summary>
    /// 入口抽象
    /// </summary>
    public abstract class AbsSSSAccessPoint
    {
        /// <summary>
        /// 数据库管理实例
        /// </summary>
        public DataBaseManager DBManager { get; init; }

        /// <summary>
        /// 日志提供器
        /// </summary>
        public AbsLoggerProvider LoggerProvider { get; init; }


        /// <summary>
        /// 构造函数
        /// </summary> 
        public AbsSSSAccessPoint(DataBaseManager dbInstance, AbsLoggerProvider loggerProvider)
        {
            DBManager = dbInstance;
            LoggerProvider = loggerProvider;
        }

        /// <summary>
        /// 当DI服务初始化时
        /// </summary>
        /// <param name="service"></param>
        public abstract void OnServiceInit(IServiceCollection service);

        /// <summary>
        /// 当系统启动完成后会触发,给予一次当前的系统上下文
        /// </summary>
        /// <param name="context"></param>
        public abstract void OnSystemStarted(AsgardContext context);

        /// <summary>
        /// 当系统正在BuildApp时
        /// </summary>
        /// <param name="builder"></param>
        public abstract void OnBuildWebApp(IApplicationBuilder builder);

        /// <summary>
        /// 排序,默认0 
        /// </summary>
        public int Order { get; set; } = 0;



        /// <summary>
        /// 系统将要关闭
        /// </summary>
        public abstract void SystemTryShutDown();
    }
}
