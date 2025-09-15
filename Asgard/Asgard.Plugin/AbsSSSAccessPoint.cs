using Asgard.Abstract;
using Asgard.Abstract.DataBase;
using Asgard.Abstract.Logger;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Asgard.Plugin
{
    /// <summary>
    /// 入口抽象
    /// </summary>
    public abstract class AbsSSSAccessPoint<ORMType>
    {
        /// <summary>
        /// 数据库管理实例
        /// </summary>
        public AbsDataBaseManager<ORMType> DBManager { get; init; }

        /// <summary>
        /// 日志提供器
        /// </summary>
        public AbsLoggerProvider LoggerProvider { get; init; }


        /// <summary>
        /// 构造函数
        /// </summary> 
        public AbsSSSAccessPoint(AbsDataBaseManager<ORMType> dbInstance, AbsLoggerProvider loggerProvider)
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
        public abstract void OnSystemStarted(AsgardContext<ORMType> context);

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
