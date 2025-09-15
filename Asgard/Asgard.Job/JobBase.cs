using Asgard.Abstract;
using Asgard.Abstract.Logger;

namespace Asgard.Job
{
    /// <summary>
    /// job抽象
    /// 记住,JOB是存在一部分自管理功能得,你的运行的错误需要自己处理,容器防止崩溃所以不记录
    /// </summary>
    [Job]
    public abstract class JobBase<OrmType> : IDisposable
    {
        /// <summary>
        /// JobID
        /// </summary>
        public string ID { get; init; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 可以给Job起一个名称,容易记忆
        /// </summary>
        public string Name { get; protected set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// 间隔,默认为空,为空时,只会触发一次,然后销毁对象,可以修改,每次运行会获取,如果是null,改成数字,则失效,只有数字改才生效
        /// </summary>
        public TimeSpan? Interval { get; protected set; } = null;

        /// <summary>
        /// 类型不可修改/修改了不会产生变化
        /// </summary>
        public JobTypeEnum JobType { get; protected set; } = JobTypeEnum.Scoped;

        /// <summary>
        /// 类型
        /// </summary>
        public JobTimerTypeEnum TimerType { get; protected set; } = JobTimerTypeEnum.Independent;

        /// <summary>
        /// 是否已经销毁
        /// </summary>
        public bool Disposed { get; protected set; }

        /// <summary>
        /// 日志提供器
        /// </summary>
        protected AbsLogger Logger { get; private set; }

        /// <summary>
        /// 取消Token
        /// </summary>
        protected readonly CancellationTokenSource _cancellToken = new();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志</param> 
        public JobBase(AbsLogger logger)
        {
            Logger = logger;
        }


        /// <summary>
        /// 析构函数
        /// </summary>
        ~JobBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// 销毁模型
        /// </summary>
        /// <param name="flag">是否是用户主动调用</param>
        protected virtual void Dispose(bool flag)
        {
            if (Disposed)
            {
                return;
            }
            Disposed = true;


            try
            {
                if (!_cancellToken.IsCancellationRequested)
                {
                    _cancellToken.Cancel();
                }
                _cancellToken.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error("抽象的通信TCP对象销毁失败.", exception: ex);
            }
        }

        /// <summary>
        /// 销毁模型
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// 启动服务 强制为任务模型
        /// </summary>
        /// <param name="context">上下文</param>
        public abstract Task Start(AsgardContext<OrmType> context);


        /// <summary>
        /// 结束 系统会并行等待这个函数
        /// </summary>
        public abstract Task Stop(AsgardContext<OrmType> context);


    }
}
