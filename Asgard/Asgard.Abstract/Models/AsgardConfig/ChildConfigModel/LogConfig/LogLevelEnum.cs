namespace Asgard.Abstract.Models.AsgardConfig.ChildConfigModel.LogConfig
{
    /// <summary>
    /// 日志级别枚举
    /// </summary>
    public enum LogLevelEnum
    {
        /// <summary>
        /// 追踪级别,可能会发生巨量日志
        /// </summary>
        Trace = 0,
        /// <summary>
        /// 调试级别
        /// </summary>
        Debug = 1,
        /// <summary>
        /// 信息级别
        /// </summary>
        Information = 2,
        /// <summary>
        /// 警告
        /// </summary>
        Warning = 3,
        /// <summary>
        /// 错误
        /// </summary>
        Error = 4,
        /// <summary>
        /// 危险,这可能会导致服务崩溃或系统异常
        /// </summary>
        Critical = 5
    }
}
