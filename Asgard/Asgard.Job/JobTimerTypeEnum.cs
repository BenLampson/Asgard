namespace Asgard.Job
{
    /// <summary>
    /// Job timer type
    /// </summary>
    public enum JobTimerTypeEnum
    {
        /// <summary>
        /// Independent timing - task timing does not count down from the previous completion
        /// </summary>
        Independent = 0,
        /// <summary>
        /// Dependent timing - task timing starts counting from the end of the previous task
        /// </summary>
        Dependent = 1
    }
}
