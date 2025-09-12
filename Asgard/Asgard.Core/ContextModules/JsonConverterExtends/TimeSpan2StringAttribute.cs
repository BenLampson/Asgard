using System.Text.Json.Serialization;

namespace Asgard.Core.ContextModules.JsonConverterExtends
{
    /// <summary>
    /// TimeSpan转字符串
    /// </summary>
    public class TimeSpan2StringAttribute : JsonConverterAttribute
    {
        private readonly string _format;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        public TimeSpan2StringAttribute(string format = "yyyy-MM-dd")
        {
            _format = format;
        }
        /// <summary>
        /// 创建一个转换器
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override JsonConverter CreateConverter(Type typeToConvert)
        {

            if (typeToConvert != typeof(TimeSpan) && typeToConvert != typeof(TimeSpan?))
            {
                throw new InvalidOperationException($"{nameof(TimeSpan2StringConverter)} 只能使用类型:{typeof(TimeSpan)}或类型:{typeof(TimeSpan?)}");
            }

            if (typeToConvert == typeof(TimeSpan))
            {
                return new TimeSpan2StringConverter();
            }
            else if (typeToConvert == typeof(TimeSpan?))
            {
                return new NullableDateTime2StringConverter(_format);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(TimeSpan2StringConverter)} 只能使用类型:{typeof(TimeSpan)}或类型:{typeof(TimeSpan?)}");
            }

        }

    }
}
