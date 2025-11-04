namespace Asgard.Job
{
    /// <summary>
    /// Job type enumeration
    /// </summary>
    public enum JobTypeEnum
    {
        /// <summary>
        /// Singleton - object is created once and never destroyed
        /// </summary>
        Singleton = 0,
        /// <summary>
        /// Scoped - a new object is created for each execution and destroyed when the job completes, recreated on next execution
        /// </summary>
        Scoped = 1
    }
}
