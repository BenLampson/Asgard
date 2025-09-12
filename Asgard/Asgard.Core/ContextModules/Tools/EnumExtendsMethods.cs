using System.ComponentModel;

namespace Asgard.Core.ContextModules.Tools
{
    /// <summary>
    /// 枚举扩展函数
    /// </summary>
    public static class EnumExtendsMethods
    {
        /// <summary>
        /// 根据某个枚举获取它对应的Description描述,如果没有该attribute则返回名称.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string GetEnumDescriptionOriginal(this Enum @this)
        {
            var name = @this.ToString();
            var field = @this.GetType().GetField(name);
            if (field == null) return name;
            var att = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute), false);
            return att == null ? field.Name : ((DescriptionAttribute)att).Description;
        }
        /// <summary>
        /// 尝试获取这个对象的某内容,如果不是枚举则为空字符串
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string GetEnumDescriptionOriginal(this object @this)
        {
            try
            {
                var name = @this.ToString();
                if (name is null)
                {
                    return string.Empty;
                }
                var field = @this.GetType().GetField(name);
                if (field == null) return name;
                var att = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute), false);
                return att == null ? field.Name : ((DescriptionAttribute)att).Description;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
