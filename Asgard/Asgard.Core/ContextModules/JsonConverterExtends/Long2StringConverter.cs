using System.Text.Json;
using System.Text.Json.Serialization;

namespace Asgard.Core.ContextModules.JsonConverterExtends
{
    /// <summary>
    /// long转字符串
    /// </summary>
    public class Long2StringConverter : JsonConverter<long>
    {
        /// <summary>
        /// 读
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var rawData = reader.GetString();
            if (long.TryParse(rawData, out var data))
            {
                return data;
            }
            return 0;
        }
        /// <summary>
        /// 写
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options) => writer?.WriteStringValue(value.ToString());
    }
}
