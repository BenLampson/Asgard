using System.Reflection;

namespace Asgard.Job
{
    /// <summary>
    /// Job service item
    /// </summary>
    internal class JobInfoItem
    {
        /// <summary>
        /// Job constructor
        /// </summary>
        public ConstructorInfo Constructor { get; set; }
        /// <summary>
        /// Job type
        /// </summary>
        public JobTypeEnum JobType { get; set; }


        /// <summary>
        /// Interval, default is null, when null it will only trigger once and then destroy the object, can be modified, retrieved each run, if null changed to number it becomes invalid, only number changes take effect
        /// </summary>
        public TimeSpan? Interval { get; set; } = null;

        /// <summary>
        /// Type
        /// </summary>
        public JobTimerTypeEnum TimerType { get; set; } = JobTimerTypeEnum.Independent;
        /// <summary>
        /// Job type full name
        /// </summary>
        public string JobTypeFullName { get; set; } = "";

        /// <summary>
        /// Instance object
        /// </summary>
        private JobBase? _jobInstance;

        /// <summary>
        /// Job instance object
        /// </summary>
        public JobBase? JobInstance
        {
            get => _jobInstance;
            set
            {
                if (JobType == JobTypeEnum.Singleton && _jobInstance is not null)
                {
                    _jobInstance = value;
                }
                else
                {
                    _jobInstance = value;
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="constructor">Target constructor</param>
        public JobInfoItem(ConstructorInfo constructor)
        {
            Constructor = constructor;
        }


    }
}
