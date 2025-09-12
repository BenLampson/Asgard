namespace Asgard.Abstract.Models.AsgardConfig.ChildConfigModel
{
    /// <summary>
    /// 默认数据库配置
    /// </summary>
    public class DefaultDBConfig
    {
        /// <summary>
        /// 本地数据库类型
        /// </summary>
        public int DbType { get; set; } = 0;
        /// <summary>
        /// 本地数据库地址
        /// </summary>
        public string DbAddress { get; set; } = "Server=127.0.0.1;port=3306;Database=Asgard; User=root;Password=!QAZ2wsx;maximumpoolsize=200;charset=utf8mb4";
        /// <summary>
        /// 本地读数据库地址
        /// </summary>
        public string[] ReadDBAddress { get; set; } = Array.Empty<string>();
    }
}
