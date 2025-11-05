using System.Text.Json;
using System.Text.Json.Serialization;

namespace Asgard.Extends.Json.Converters
{
    /// <summary>
    /// 日志转时间的转换器
    /// </summary>
    public class DateTime2StringConverter : JsonConverter<DateTime>
    {

        private readonly string _format;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="format"></param>
        public DateTime2StringConverter(string format)
        {
            _format = format;
        }
        /// <summary>
        /// 读
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var raw = reader.GetString();
            if (DateTime.TryParse(raw, out var data))
            {
                return data;
            }
            return DateTime.MinValue;
        }
        /// <summary>
        /// 写
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => writer?.WriteStringValue(value.ToString(_format));
    }
}
