using System.Text;
using System.Text.Json;

using Asgard.Core.ContextModules.Tools;

using static Asgard.Core.ContextModules.JsonConverterExtends.CommonSerializerOptions;

namespace Asgard.Core.ContextModules.JsonConverterExtends
{
    /// <summary>
    /// json压缩级别
    /// </summary>
    public enum JsonCompressLevel
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 普通
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 高
        /// </summary>
        High = 2
    }
    /// <summary>
    /// Json扩展函数
    /// </summary>
    public static class JsonExtends
    {
        /// <summary>
        /// 获取一个Json配置
        /// </summary>
        /// <param name="camelCase"></param>
        /// <param name="withChinese"></param>
        /// <param name="nameCaseInsensitive"></param>
        /// <param name="addDateTime2String"></param>
        /// <param name="addLong2String"></param>
        /// <returns></returns>
        private static JsonSerializerOptions GetOptions(bool camelCase = true,
            bool withChinese = true,
            bool nameCaseInsensitive = true,
            bool addDateTime2String = true,
            bool addLong2String = true)
        {

            JsonSerializerOptions options = new();
            if (camelCase && withChinese && nameCaseInsensitive)
            {
                options = new(CamelCaseChineseNameCaseInsensitive);
            }
            else if (!camelCase && withChinese && nameCaseInsensitive)
            {
                options = new(ChineseNameCaseInsensitive);
            }
            else if (!camelCase && !withChinese && nameCaseInsensitive)
            {
                options = new(NameCaseInsensitive);
            }
            else if (camelCase && !withChinese && nameCaseInsensitive)
            {
                options = new(CamelCaseNameCaseInsensitive);
            }
            else if (camelCase && !withChinese && !nameCaseInsensitive)
            {
                options = new(CamelCase);
            }
            else if (camelCase && withChinese && !nameCaseInsensitive)
            {
                options = new(CamelCaseChinese);
            }
            if (addDateTime2String)
            {
                options.Converters.Add(new DateTime2StringConverter("yyyy-MM-dd HH:mm"));
            }
            if (addLong2String)
            {
                options.Converters.Add(new Long2StringConverter());
            }
            return options;
        }

        /// <summary>
        /// 尝试把一个对象使用json序列化一下 UTF8编码的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="compress"></param>
        /// <param name="camelCase"></param>
        /// <param name="withChinese"></param>
        /// <param name="nameCaseInsensitive"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static byte[]? GetBytes<T>(this T target, JsonCompressLevel compress = JsonCompressLevel.None, bool camelCase = true, bool withChinese = true, bool nameCaseInsensitive = true, JsonSerializerOptions? options = null) where T : class
        {
            if (target == null)
            {
                return default;
            }
            if (options == null)
            {
                options = GetOptions(camelCase, withChinese, nameCaseInsensitive);
            }
            switch (compress)
            {
                case JsonCompressLevel.Normal:
                    return BrotliUTF8.Compress(JsonSerializer.Serialize(target, options));
                case JsonCompressLevel.High:
                    return BrotliUTF8.CompressHightLevel(JsonSerializer.Serialize(target, options));
                case JsonCompressLevel.None:
                default:
                    return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(target, options));
            }
        }


        /// <summary>
        /// 把一个UTF8编码的byte数组尝试用json序列化为一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="compress"></param>
        /// <param name="camelCase"></param>
        /// <param name="withChinese"></param>
        /// <param name="nameCaseInsensitive"></param>
        /// <returns></returns>
        public static T? GetObject<T>(this byte[] source, JsonCompressLevel compress = JsonCompressLevel.None, bool camelCase = true, bool withChinese = true, bool nameCaseInsensitive = true) where T : class
        {
            if (source == null)
            {
                return default;
            }
            switch (compress)
            {
                case JsonCompressLevel.High:
                case JsonCompressLevel.Normal:
                    return JsonSerializer.Deserialize<T>(BrotliUTF8.GetString(source), GetOptions(camelCase, withChinese, nameCaseInsensitive));
                case JsonCompressLevel.None:
                default:
                    return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(source), GetOptions(camelCase, withChinese, nameCaseInsensitive));
            }

        }
    }
}