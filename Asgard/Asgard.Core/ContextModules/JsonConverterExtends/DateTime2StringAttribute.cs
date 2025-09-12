using System.Text.Json.Serialization;

namespace Asgard.Core.ContextModules.JsonConverterExtends
{
    /// <summary>
    /// 日志转时间的字符串
    /// </summary>
    public class DateTime2StringAttribute : JsonConverterAttribute
    {
        private readonly string _format;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="format"></param>
        public DateTime2StringAttribute(string format = "yyyy-MM-dd")
        {
            _format = format;
        }
        /// <summary>
        /// 转换器
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override JsonConverter CreateConverter(Type typeToConvert)
        {

            if (typeToConvert != typeof(DateTime) && typeToConvert != typeof(DateTime?))
            {
                throw new InvalidOperationException($"{nameof(DateTime2StringAttribute)} 只能使用类型:{typeof(DateTime)}或类型:{typeof(DateTime?)}");
            }

            if (typeToConvert == typeof(DateTime))
            {
                return new DateTime2StringConverter(_format);
            }
            else if (typeToConvert == typeof(DateTime?))
            {
                return new NullableDateTime2StringConverter(_format);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(DateTime2StringAttribute)} 只能使用类型:{typeof(DateTime)}或类型:{typeof(DateTime?)}");
            }

        }

    }
}
