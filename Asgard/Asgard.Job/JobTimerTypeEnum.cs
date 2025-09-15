namespace Asgard.Job
{
    /// <summary>
    /// 任务计时器类型
    /// </summary>
    public enum JobTimerTypeEnum
    {
        /// <summary>
        /// 独立计时,这意味着你的任务计时并不是从上一次结束开始倒计时
        /// </summary>
        Independent = 0,
        /// <summary>
        /// 依赖计时,这意味着你的任务计时是从上一次任务结束开始计时
        /// </summary>
        Dependent = 1
    }
}
