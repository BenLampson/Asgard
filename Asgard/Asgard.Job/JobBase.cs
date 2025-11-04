using Asgard.Abstract;
using Asgard.Abstract.Logger;

namespace Asgard.Job
{
    /// <summary>
    /// Job abstraction
    /// Remember, JOB has some self-management functionality, you need to handle your own runtime errors, the container doesn't log them to prevent crashes
    /// </summary>
    [Job]
    public abstract class JobBase : IDisposable
    {
        /// <summary>
        /// JobID
        /// </summary>
        public string ID { get; init; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// Can give the Job a name for easy memory
        /// </summary>
        public string Name { get; protected set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Interval, default is null, when null it will only trigger once and then destroy the object, can be modified, retrieved each run, if null changed to number it becomes invalid, only number changes take effect
        /// </summary>
        public TimeSpan? Interval { get; protected set; } = null;

        /// <summary>
        /// Type cannot be modified/modifying will have no effect
        /// </summary>
        public JobTypeEnum JobType { get; protected set; } = JobTypeEnum.Scoped;

        /// <summary>
        /// Type
        /// </summary>
        public JobTimerTypeEnum TimerType { get; protected set; } = JobTimerTypeEnum.Independent;

        /// <summary>
        /// Whether it has been disposed
        /// </summary>
        public bool Disposed { get; protected set; }

        /// <summary>
        /// Logger provider
        /// </summary>
        protected AbsLogger Logger { get; private set; }

        /// <summary>
        /// Cancellation token
        /// </summary>
        protected readonly CancellationTokenSource _cancellToken = new();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        public JobBase(AbsLogger logger)
        {
            Logger = logger;
        }


        /// <summary>
        /// Destructor
        /// </summary>
        ~JobBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposal model
        /// </summary>
        /// <param name="flag">Whether called by user actively</param>
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
                Logger.Error("Abstract communication TCP object disposal failed.", exception: ex);
            }
        }

        /// <summary>
        /// Disposal model
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Start service - forced as job model
        /// </summary>
        /// <param name="context">Context</param>
        public abstract Task Start(AsgardContext context);


        /// <summary>
        /// Stop - system will wait for this function in parallel
        /// </summary>
        public abstract Task Stop(AsgardContext context);


    }
}
