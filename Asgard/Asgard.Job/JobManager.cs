using Asgard.Abstract;
using Asgard.Abstract.Job;
using Asgard.Abstract.Logger;

namespace Asgard.Job
{
    /// <summary>
    /// Job manager
    /// </summary>
    public class JobManager : AbsJobManager
    {

        /// <summary>
        /// All job objects
        /// </summary>
        private readonly List<JobInfoItem> _allJobItems = new();



        /// <summary>
        /// Default constructor
        /// </summary>
        public JobManager(AbsLoggerProvider provider) : base(provider)
        {
        }


        /// <summary>
        /// Push a new job content
        /// </summary>
        /// <param name="jobType">Job service type</param>
        public override void PushNewJobInfo(Type jobType)
        {
            if (typeof(JobBase).IsAssignableFrom(jobType))
            {
                var constructor = jobType.GetConstructor(new Type[] { typeof(AbsLogger) });
                if (constructor is null)
                {
                    _logger.Information($"Worker's default constructor not found, not loading!");
                    return;
                }

                var loggerInstance = _provider.CreateLogger(jobType.FullName ?? jobType.Name);
                try
                {
                    var instance = constructor.Invoke(new object[] { loggerInstance });
                    if (instance is null)
                    {
                        _logger.Information($"Failed to create worker instance, not loading!");
                        return;
                    }
                    if (instance is not JobBase jobBase)
                    {
                        _logger.Information($"Worker does not inherit base class AbsJobBase, not loading!");
                        return;
                    }
                    var item = new JobInfoItem(constructor)
                    {
                        JobTypeFullName = jobType.FullName ?? jobType.Name,
                        Interval = jobBase.Interval,
                        JobType = jobBase.JobType,
                        TimerType = jobBase.TimerType
                    };
                    if (jobBase.JobType == JobTypeEnum.Singleton)
                    {
                        item.JobInstance = jobBase;
                    }
                    _allJobItems.Add(item);
                    _logger.Information($"Found worker:{jobBase.Name}" +
                        $" Type:{Enum.GetName(jobBase.JobType)}" +
                        $" Timer type:{Enum.GetName(jobBase.TimerType)}" +
                        $" Interval:{(jobBase.Interval == null ? "Run once" : jobBase.Interval.ToString())}");
                }
                catch (Exception ex)
                {
                    _logger.Information($"Error creating worker instance!", exception: ex);
                    return;
                }

            }
        }

        /// <summary>
        /// Start service
        /// </summary>
        public override void Start()
        {
            if (CreateContextAction is null)
            {
                return;
            }
            _allJobItems.ForEach(item =>
            {
                _ = Task.Run(async () =>
                {
                    while (!_cancellationToken.Token.IsCancellationRequested)
                    {
                        if (item.JobType == JobTypeEnum.Scoped)
                        {
                            var loggerInstance = _provider.CreateLogger(item.JobTypeFullName);
                            var instance = item.Constructor.Invoke(new object[] { loggerInstance });

                            if (instance is not JobBase baseInstance)
                            {
                                _logger.Information($"Worker {item.JobTypeFullName} instance creation complete, but it's not AbsJobBase, skipping execution!");
                                return;
                            }
                            item.JobInstance = baseInstance;

                            if (item.JobInstance is null)
                            {
                                _logger.Information($"Failed to create worker {item.JobTypeFullName} instance, skipping execution!");
                                return;
                            }
                        }

                        if (item.JobInstance is null)
                        {
                            _logger.Information($"Worker {item.JobTypeFullName} instance is null, skipping execution.");
                            break;
                        }
                        if (item.TimerType == JobTimerTypeEnum.Independent)
                        {
                            _ = Task.Run(async () =>
                            {
                                using var context = CreateContextAction();
                                await item.JobInstance.Start(context);
                                if (item.JobType == JobTypeEnum.Scoped)
                                {
                                    item.JobInstance.Dispose();
                                }
                            });
                        }
                        else
                        {
                            using var context = CreateContextAction();
                            await item.JobInstance.Start(context);

                            if (item.JobType == JobTypeEnum.Scoped)
                            {
                                item.JobInstance.Dispose();
                            }
                        }
                        if (item.Interval is null)
                        {
                            break;
                        }
                        else if (item.JobType == JobTypeEnum.Scoped)
                        {
                            await Task.Delay(item.Interval.Value, _cancellationToken.Token);
                        }
                        else if (item.JobInstance.Interval is null)
                        {
                            break;
                        }
                        else if (item.JobType == JobTypeEnum.Singleton && item.JobInstance is not null)
                        {
                            await Task.Delay(item.JobInstance.Interval.Value, _cancellationToken.Token);
                        }


                    }
                }, _cancellationToken.Token);
            });
        }

        /// <summary>
        /// Stop service
        /// </summary>
        public override void Stop(AsgardContext context)
        {
            _cancellationToken.Cancel();
            _allJobItems.ForEach(item =>
            {
                try
                {
                    _ = item.JobInstance?.Stop(context);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Stop job error.", exception: ex, eventID: context.EventID);
                }
            });
        }
    }
}
