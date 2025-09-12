namespace Asgard.Abstract.Models.AsgardConfig.ChildConfigModel
{
    /// <summary>
    /// 认证配置
    /// </summary>
    public class AuthConfig
    {
        //string jwtKey, string issuer, string audience, string tdListAesKey, string tdListAesIV
        /// <summary>
        /// JWT认证key
        /// </summary>
        public string JwtKey { get; set; } = string.Empty;

        /// <summary>
        /// 颁发者
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// 收听人
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// 其他信息AES密钥
        /// </summary>
        public string AesKey { get; set; } = string.Empty;
        /// <summary>
        /// 其它信息AESIV
        /// </summary>
        public string AesIV { get; set; } = string.Empty;
    }
}
