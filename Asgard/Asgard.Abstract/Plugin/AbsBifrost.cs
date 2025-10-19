using Asgard.Abstract.DataBase;
using Asgard.Abstract.Logger;

namespace Asgard.Abstract.Plugin
{
    /// <summary>
    /// 入口抽象
    /// </summary>
    public abstract class AbsBifrost
    {
        /// <summary>
        /// 数据库管理实例
        /// </summary>
        public AbsDataBaseManager? DBManager { get; init; }

        /// <summary>
        /// 日志提供器
        /// </summary>
        public AbsLoggerProvider? LoggerProvider { get; init; }


        /// <summary>
        /// 构造函数
        /// </summary> 
        public AbsBifrost(AbsDataBaseManager? dbInstance, AbsLoggerProvider? loggerProvider)
        {
            DBManager = dbInstance;
            LoggerProvider = loggerProvider;
        }


        /// <summary>
        /// 当系统启动完成后会触发,给予一次当前的系统上下文
        /// </summary>
        /// <param name="context"></param>
        public abstract void OnSystemStarted(AsgardContext context);


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
