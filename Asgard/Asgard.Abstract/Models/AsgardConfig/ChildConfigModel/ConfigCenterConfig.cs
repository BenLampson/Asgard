namespace Asgard.Abstract.Models.AsgardConfig.ChildConfigModel
{
    /// <summary>
    /// 配置中心的配置信息
    /// </summary>
    public class ConfigCenterConfig
    {
        /// <summary>
        /// 配置中心IP地址
        /// </summary>
        public string ConfigCenter { get; set; } = "127.0.0.1";
        /// <summary>
        /// 配置中心端口
        /// </summary>
        public ushort ConfigCenterPort { get; set; } = 12341;
        /// <summary>
        /// 是否自宿主
        /// </summary>
        public bool SelfHostConfigCenter { get; set; }

        /// <summary>
        /// 脱离配置中心体系,意味着这个东西是独立服务
        /// </summary>
        public bool WithOutConfigCenter { get; set; }
    }
}
