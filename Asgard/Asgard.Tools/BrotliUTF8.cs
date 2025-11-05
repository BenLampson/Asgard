using System.Text;

namespace Asgard.Tools
{
    /// <summary>
    /// brotli utf8 encoding
    /// </summary>
    public static class BrotliUTF8
    {
        /// <summary>
        /// compress data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Compress(string data)
        {
            return Compress(Encoding.UTF8.GetBytes(data));
        }
        /// <summary>
        /// use hight level compress data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] CompressHightLevel(string data)
        {
            return CompressHightLevel(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// compress data
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] Compress(ReadOnlySpan<byte> source)
        {
            using var outStream = new MemoryStream();
            using var brotliStream = new System.IO.Compression.BrotliStream(outStream, System.IO.Compression.CompressionLevel.Fastest);
            brotliStream.Write(source);
            brotliStream.Close();
            return outStream.ToArray();
        }
        /// <summary>
        /// use hight level compress data
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] CompressHightLevel(ReadOnlySpan<byte> source)
        {
            using var outStream = new MemoryStream();
            using var brotliStream = new System.IO.Compression.BrotliStream(outStream, System.IO.Compression.CompressionLevel.Optimal);
            brotliStream.Write(source);
            brotliStream.Close();
            return outStream.ToArray();
        }


        /// <summary>
        /// decompress data
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] Decompress(ReadOnlySpan<byte> source)
        {
            using var inputStream = new MemoryStream();
            inputStream.Write(source);
            inputStream.Flush();
            inputStream.Position = 0;
            using var outStream = new MemoryStream();
            using var brotliStream = new System.IO.Compression.BrotliStream(inputStream, System.IO.Compression.CompressionMode.Decompress);
            brotliStream.CopyTo(outStream);
            return outStream.ToArray();
        }

        /// <summary>
        /// get string with decompress
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetString(ReadOnlySpan<byte> source)
        {
            return Encoding.UTF8.GetString(Decompress(source));
        }
    }
}
