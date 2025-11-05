namespace Asgard.Extends.AspNetCore.ApiModels
{
    /// <summary>
    /// Pagination request base class
    /// </summary>
    public abstract class PageRequestBase
    {
        /// <summary>
        /// Page index
        /// </summary>
        public int? PageIndex { get; set; } = 1;
        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// Current ID for waterfall, priority lower than PageIndex when both exist
        /// </summary>
        public long? StartID { get; set; }
    }
}
