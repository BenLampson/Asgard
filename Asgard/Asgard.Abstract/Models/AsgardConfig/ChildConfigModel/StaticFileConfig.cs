namespace Asgard.Abstract.Models.AsgardConfig.ChildConfigModel
{
    /// <summary>
    /// 静态文件配置
    /// </summary>
    public class StaticFileConfig
    {
        /// <summary>
        /// 目标文件地址
        /// </summary>
        public string Folder { get; set; } = "staticFiles";
        /// <summary>
        /// 启用静态文件
        /// </summary>
        public bool UseStaticFolder { get; set; }

        /// <summary>
        /// 请求前缀
        /// </summary>
        public string Prefix { get; set; } = string.Empty;
        /// <summary>
        /// 自动重定向,不填写则不跳转
        /// </summary>
        public string AutoRedirect { get; set; } = string.Empty;
    }
}
