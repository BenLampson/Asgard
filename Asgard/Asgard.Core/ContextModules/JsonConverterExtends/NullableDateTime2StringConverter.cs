using System.Text.Json;
using System.Text.Json.Serialization;

namespace Asgard.Core.ContextModules.JsonConverterExtends
{
    /// <summary>
    /// 可控时间转字符串
    /// </summary>
    public class NullableDateTime2StringConverter : JsonConverter<DateTime?>
    {

        private readonly string _format;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="format">时间类型</param>
        public NullableDateTime2StringConverter(string format)
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
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                var raw = reader.GetString();
                if (DateTime.TryParse(raw, out var data))
                {
                    return data;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 写
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value == null)
            {

            }
            else
            {
                writer?.WriteStringValue(value.Value.ToString(_format));
            }
        }
    }
}
