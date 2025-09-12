using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Asgard.Extends.Json.Converters
{
    /// <summary>
    /// TimeSpan转字符串换
    /// </summary>
    public class TimeSpan2StringConverter : JsonConverter<TimeSpan>
    {
        /// <summary>
        /// 读
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new NotSupportedException();
            if (typeToConvert != typeof(TimeSpan))
                throw new NotSupportedException();
            return TimeSpan.ParseExact(reader.GetString() ?? "", "c", CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// 写
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("c", CultureInfo.InvariantCulture));
        }
    }
}
