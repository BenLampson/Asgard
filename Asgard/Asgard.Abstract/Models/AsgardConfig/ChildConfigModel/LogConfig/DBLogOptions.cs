namespace Asgard.Abstract.Models.AsgardConfig.ChildConfigModel.LogConfig
{
    /// <summary>
    /// 数据库日志配置信息
    /// </summary>
    public class DBLogOptions
    {
        /// <summary>
        /// 启用数据库日志
        /// </summary>
        public bool EnableDBLog { get; set; } = false;

        /// <summary>
        /// 数据库地址,为空则不保存
        /// </summary>
        public string DbAddress { get; set; } = string.Empty;

        /// <summary>
        /// 数据库类型
        /// </summary>
        public int DbType { get; set; }

        /// <summary>
        /// 指定table名,用来拆分默认都是LogInfo
        /// </summary>
        public string TableName { get; set; } = "LogInfo";
    }
}
