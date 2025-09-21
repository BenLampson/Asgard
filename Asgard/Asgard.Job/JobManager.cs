using Asgard.Abstract;
using Asgard.Abstract.Logger;

namespace Asgard.Job
{
    /// <summary>
    /// 任务管理器
    /// </summary>
    public class JobManager
    {
        /// <summary>
        /// 日志提供器
        /// </summary>
        private readonly AbsLoggerProvider _provider;
        /// <summary>
        /// 日志对象
        /// </summary>
        private readonly AbsLogger _logger;

        /// <summary>
        /// 所有的任务对象
        /// </summary>
        private readonly List<JobInfoItem> _allJobItems = new();

        /// <summary>
        /// 取消Token
        /// </summary>
        private readonly CancellationTokenSource _cancellationToken = new();

        /// <summary>
        /// 创建上下文函数
        /// </summary>
        public Func<AsgardContext>? CreateContextAction;

        /// <summary>
        /// 默认构造
        /// </summary>
        public JobManager(AbsLoggerProvider provider)
        {
            _provider = provider;
            _logger = provider.CreateLogger<JobManager>();
        }




        /// <summary>
        /// 推送一个新的任务内容
        /// </summary>
        /// <param name="jobType">job服务的类型</param> 
        public void PushNewJobInfo(Type jobType)
        {
            if (typeof(JobBase).IsAssignableFrom(jobType))
            {
                var constructor = jobType.GetConstructor(new Type[] { typeof(AbsLogger) });
                if (constructor is null)
                {
                    _logger.Information($"工作器的默认构造函数未找到,不加载!");
                    return;
                }

                var loggerInstance = _provider.CreateLogger(jobType.FullName ?? jobType.Name);
                try
                {
                    var instance = constructor.Invoke(new object[] { loggerInstance });
                    if (instance is null)
                    {
                        _logger.Information($"创建工作器实例失败,不加载!");
                        return;
                    }
                    if (instance is not JobBase jobBase)
                    {
                        _logger.Information($"工作器没有继承基类AbsJobBase,不加载!");
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
                    _logger.Information($"找到工作器:{jobBase.Name}" +
                        $" 类型:{Enum.GetName(jobBase.JobType)}" +
                        $" 计时器类型:{Enum.GetName(jobBase.TimerType)}" +
                        $" 间隔:{(jobBase.Interval == null ? "只运行一次" : jobBase.Interval.ToString())}");
                }
                catch (Exception ex)
                {
                    _logger.Information($"创建工作器实例报错!", exception: ex);
                    return;
                }

            }
        }

        /// <summary>
        /// 开始服务
        /// </summary>
        public void Start()
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
                                _logger.Information($"创建工作器:{item.JobTypeFullName}实例完成,但是其并不是AbsJobBase, 跳过执行!");
                                return;
                            }
                            item.JobInstance = baseInstance;

                            if (item.JobInstance is null)
                            {
                                _logger.Information($"创建工作器:{item.JobTypeFullName} 实例失败,跳过执行!");
                                return;
                            }
                        }

                        if (item.JobInstance is null)
                        {
                            _logger.Information($"工作器:{item.JobTypeFullName}实例为空,跳过执行.");
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
        /// 停止服务
        /// </summary>
        public void Stop(AsgardContext context)
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
