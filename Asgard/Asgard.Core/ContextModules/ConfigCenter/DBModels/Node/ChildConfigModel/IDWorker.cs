namespace Asgard.Core.ContextModules.ConfigCenter.DBModels.Node.ChildConfigModel
{
    /// <summary>
    /// 雪花ID生成器
    /// </summary>
    public class IDWorker
    {
        /// <summary>
        /// 雪花ID的workID
        /// </summary>
        public long WorkID { get; set; } = 1;
        /// <summary>
        /// 雪花ID的数据中心ID
        /// </summary>
        public long DataCenterID { get; set; } = 1;
    }
}
