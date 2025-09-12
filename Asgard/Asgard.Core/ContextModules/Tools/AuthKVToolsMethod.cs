using System.Security.Cryptography;

namespace Asgard.Core.ContextModules.Tools
{
    /// <summary>
    /// 帮助函数
    /// </summary>
    public class AuthKVToolsMethod
    {
        /// <summary>
        /// 创建新的AES加密key与IV
        /// </summary>
        /// <returns></returns>
        public static (string key, string iv) CreateNewAesKeyAndVi()
        {
            var aes = Aes.Create();
            return (Convert.ToBase64String(aes.Key), Convert.ToBase64String(aes.IV));
        }

        /// <summary>
        /// 创建新的RSA密钥
        /// </summary>
        /// <returns></returns>
        public static string CreateNewHMACSHA256Key()
        {
            using (HMACSHA256 hmac = new HMACSHA256())
            {
                byte[] key = hmac.Key;
                return Convert.ToBase64String(key);
            }
        }
    }
}
