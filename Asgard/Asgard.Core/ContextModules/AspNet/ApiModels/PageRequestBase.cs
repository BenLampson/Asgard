namespace Asgard.Core.ContextModules.AspNet.ApiModels
{
    /// <summary>
    /// 分页请求基础类
    /// </summary>
    public abstract class PageRequestBase
    {
        /// <summary>
        /// 页下标
        /// </summary>
        public int? PageIndex { get; set; } = 1;
        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// 当前ID,做瀑布流  并存的话优先级低于PageIndex
        /// </summary>
        public long? StartID { get; set; }
    }
}
