using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Asgard.Abstract.Plugin
{
    internal class FileOperations
    {
        /// <summary>
        /// 计算文件sha256
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <returns></returns>
        public static string? SHA256Encrypt(string filePath)
        {
            using var sha256 = SHA256.Create();

            try
            {
                FileStream fileStream = File.Open(filePath, FileMode.Open);
                fileStream.Position = 0;
                byte[] hashValue = sha256.ComputeHash(fileStream);
                fileStream.Close();
                return PrintByteArray(hashValue);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 以可读格式显示字节数组
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static string PrintByteArray(byte[] array)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                _ = sb.Append($"{array[i]:X2}");
            }
            return sb.ToString();
        }


    }
}
