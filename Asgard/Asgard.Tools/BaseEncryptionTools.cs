using System.Security.Cryptography;
using System.Text;

namespace Asgard.Tools
{
    /// <summary>
    /// 基础的加密工具类
    /// </summary>
    public class BaseEncryptionTools
    {
        private static byte[] _key = new byte[] { 56, 189, 84, 192, 142, 103, 87, 65, 128, 225, 57, 207, 66, 83, 73, 219, 41, 77, 236, 240, 252, 182, 18, 54, 96, 44, 121, 65, 101, 198, 131, 163 };
        private static byte[] _iv = new byte[] { 108, 251, 200, 139, 42, 249, 49, 21, 162, 8, 93, 70, 17, 139, 121, 253 };
        private static Aes _aes = Aes.Create();
        private static MD5 _md5 = MD5.Create();
        /// <summary>
        /// 设置工具类的AES密钥
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        public static void SetKeyAndIV(string key, string iv)
        {
            _aes.Dispose();
            _md5.Dispose();
            _aes = Aes.Create();
            _md5 = MD5.Create();
            _key = Convert.FromBase64String(key);
            _iv = Convert.FromBase64String(iv);

            _aes.Key = _key;
            _aes.IV = _iv;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string Md5(string txt)
        {
            byte[] sor = Encoding.UTF8.GetBytes(txt);
            byte[] result = _md5.ComputeHash(sor);
            StringBuilder strbul = new(40);
            for (int i = 0; i < result.Length; i++)
            {
                //加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
                _ = strbul.Append(result[i].ToString("x2"));
            }
            return strbul.ToString();
        }

        /// <summary>
        /// 加密一个字符串
        /// </summary>
        /// <param name="plainText">原始文字</param>
        /// <returns>加密结果</returns>
        public static string? EncryptStringToString_Aes(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
            {
                return null;
            }
            try
            {
                byte[] encrypted;

                ICryptoTransform encryptor = _aes.CreateEncryptor(_aes.Key, _aes.IV);
                using MemoryStream msEncrypt = new();
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                using StreamWriter swEncrypt = new(csEncrypt);
                swEncrypt.Write(plainText);
                swEncrypt.Flush();
                swEncrypt.Close();
                encrypted = msEncrypt.ToArray();



                return BitConverter.ToString(encrypted).Replace("-", "");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 解密一个字符串
        /// </summary>
        /// <param name="cipherText">原始文字</param>
        /// <returns>解密结果</returns>
        public static string? DecryptStringFromStr_Aes(string cipherText)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
            {
                return null;
            }

            try
            {

                byte[] buf1 = new byte[cipherText.Length / 2];
                for (int i = 0; i < cipherText.Length; i += 2)
                {
                    buf1[i / 2] = Convert.ToByte(cipherText[i..(i + 2)], 16);
                }
                ICryptoTransform decryptor = _aes.CreateDecryptor(_aes.Key, _aes.IV);
                using MemoryStream msDecrypt = new(buf1);
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt);
                return srDecrypt.ReadToEnd();
            }
            catch
            {

            }
            return null;
        }
    }
}
