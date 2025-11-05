namespace Asgard.Abstract.Models.AsgardUserInfo
{
    /// <summary>
    /// 商户数据库信息
    /// </summary>
    public class TDBItem
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnStr { get; init; } = string.Empty;
        /// <summary>
        /// 数据库类型
        /// </summary>
        public int Type { get; init; }
        /// <summary>
        /// 数据库key
        /// </summary>
        public string Key { get; init; } = string.Empty;

        /// <summary>
        /// 读库信息
        /// </summary>
        public string[] ReadDB { get; init; } = Array.Empty<string>();
    }
}
