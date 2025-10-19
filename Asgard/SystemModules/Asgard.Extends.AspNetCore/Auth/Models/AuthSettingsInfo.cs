namespace Asgard.Extends.AspNetCore.Auth.Models
{
    /// <summary>
    /// 权限系统配置信息
    /// </summary>
    public class AuthSettingsInfo
    {
        /// <summary>
        /// 权限中心地址
        /// </summary>
        public string AuthCenterUrl { get; init; } = "";
        /// <summary>
        /// 离线存储时长,不管再怎么离线,它也应该有一个度
        /// </summary>
        public int OfflineCacheSeconds { get; init; } = 60 * 60 * 8;

    }
}
