namespace Asgard.Core.ContextModules.Job
{
    /// <summary>
    /// 任务类型枚举
    /// </summary>
    public enum JobTypeEnum
    {
        /// <summary>
        /// 单例运行,创建一次对象永远不销毁
        /// </summary>
        Singleton = 0,
        /// <summary>
        /// 范围内,代指每次运行都会创建一次对象,只要任务执行完毕,就会销毁,后面再次创建
        /// </summary>
        Scoped = 1
    }
}
