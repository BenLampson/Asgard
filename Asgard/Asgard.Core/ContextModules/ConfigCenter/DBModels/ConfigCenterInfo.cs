//namespace Asgard.Core.ContextModules.ConfigCenter.DBModels
//{
//    /// <summary>
//    /// 配置中心配置
//    /// </summary>
//    public class ConfigCenterInfo
//    {
//        /// <summary>
//        /// 配置中心IP地址
//        /// </summary>
//        public string ConfigCenter { get; set; } = "127.0.0.1";
//        /// <summary>
//        /// 配置中心端口
//        /// </summary>
//        public ushort ConfigCenterPort { get; set; } = 12341;
//        /// <summary>
//        /// 是否自宿主
//        /// </summary>
//        public bool SelfHostConfigCenter { get; set; }
//        /// <summary>
//        /// redis连接字符串
//        /// </summary>
//        public string RedisStr { get; set; } = "";
//        /// <summary>
//        /// 数据库主库连接字符串
//        /// </summary>
//        public string WriteDB { get; set; } = "";
//        /// <summary>
//        /// 数据库类型
//        /// </summary>
//        public int DBType { get; set; } = 0;
//        /// <summary>
//        /// 读库地址
//        /// </summary>
//        public string[] ReadDB { get; set; } = Array.Empty<string>();
//    }
//}
