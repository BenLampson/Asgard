using Asgard.Abstract.Logger;

namespace Asgard.Abstract.Job
{
    /// <summary>
    /// 任务管理器
    /// </summary>
    public abstract class AbsJobManager
    {
        /// <summary>
        /// 日志提供器
        /// </summary>
        protected readonly AbsLoggerProvider _provider;

        /// <summary>
        /// 日志对象
        /// </summary>
        protected readonly AbsLogger _logger;

        /// <summary>
        /// 取消Token
        /// </summary>
        protected readonly CancellationTokenSource _cancellationToken = new();

        /// <summary>
        /// 创建上下文函数
        /// </summary>
        public Func<AsgardContext>? CreateContextAction;

        /// <summary>
        /// 默认构造
        /// </summary>
        public AbsJobManager(AbsLoggerProvider provider)
        {
            _provider = provider;
            _logger = provider.CreateLogger<AbsJobManager>();
        }




        /// <summary>
        /// 推送一个新的任务内容
        /// </summary>
        /// <param name="jobType">job服务的类型</param> 
        public abstract void PushNewJobInfo(Type jobType);

        /// <summary>
        /// 开始服务
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// 停止服务
        /// </summary>
        public abstract void Stop(AsgardContext context);
    }
}
