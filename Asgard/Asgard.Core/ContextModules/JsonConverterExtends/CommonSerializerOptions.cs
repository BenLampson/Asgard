using System.Text.Json;
using System.Text.Unicode;

namespace Asgard.Core.ContextModules.JsonConverterExtends
{
    /// <summary>
    /// 公共序列化配置
    /// </summary>
    public static class CommonSerializerOptions
    {
        /// <summary>
        /// 支持驼峰
        /// </summary>
        public static readonly JsonSerializerOptions CamelCase = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// 名字忽略的选项
        /// </summary>
        public static readonly JsonSerializerOptions NameCaseInsensitive = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// 支持中文的选项
        /// </summary>
        public static readonly JsonSerializerOptions ChineseOptions = new()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        /// <summary>
        /// 驼峰+中文
        /// </summary>
        public static readonly JsonSerializerOptions CamelCaseChinese = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
        };
        /// <summary>
        /// 驼峰+忽略大小写
        /// </summary>
        public static readonly JsonSerializerOptions CamelCaseNameCaseInsensitive = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        /// <summary>
        /// 中文+忽略大小写
        /// </summary>
        public static readonly JsonSerializerOptions ChineseNameCaseInsensitive = new()
        {
            PropertyNameCaseInsensitive = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        /// <summary>
        /// 驼峰+中文+大小写
        /// </summary>
        public static readonly JsonSerializerOptions CamelCaseChineseNameCaseInsensitive = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
        };

    }
}
