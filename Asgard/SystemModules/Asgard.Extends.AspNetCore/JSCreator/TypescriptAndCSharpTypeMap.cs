namespace Asgard.Extends.AspNetCore.JSCreator
{
    /// <summary>
    /// TypeScript 与 C# 类型映射工具
    /// </summary>
    public class TypescriptAndCSharpTypeMap
    {
        /// <summary>
        /// 类型映射,key是C#的,value是TS的
        /// </summary>
        public static Dictionary<string, string> Mapper = new()
        {
            {"int64","string"},
            {"int32","number"},
            {"uint16","number"},
            {"double","number"},
            {"float","number"},
            {"string","string"},
            {"decimal","number"},
            {"datetime","string"},
            {"iformfile","File"},
            {"string[]","Array<string>"},
            {"int32[]","Array<number>"},
            {"uint16[]","Array<number>"},
            {"double[]","Array<number>"},
            {"float[]","Array<number>"},
            {"int64[]","Array<string>"},
            {"decimal[]","Array<number>"},
            {"datetime[]","Array<string>"}
        };
        /// <summary>
        /// 获取类型字符串及是否为数组
        /// </summary>
        /// <param name="rawTargetType">原始目标类型</param>
        /// <param name="modelNamespace">模型命名空间</param>
        /// <param name="res">类型字符串</param>
        /// <param name="isArray">是否为数组</param>
        public static void GetTypeString(Type rawTargetType, string modelNamespace, out string res, out bool isArray)
        {
            _ = modelNamespace;
            var targetType = rawTargetType;
            if (rawTargetType.IsGenericType)
            {
                isArray = rawTargetType.GenericTypeArguments[0].IsArrayOrList();
                targetType = rawTargetType.GenericTypeArguments[0];
            }
            else
            {
                isArray = rawTargetType.IsArrayOrList();
            }
            if (isArray)
            {
                string detailTypeName;

                if (targetType.GenericTypeArguments.Length == 0)
                {
                    detailTypeName = targetType.Name;
                }
                else
                {
                    detailTypeName = targetType.GenericTypeArguments[0].Name;
                }
                if (Mapper.ContainsKey(detailTypeName.ToLower()))
                {
                    res = $"Array<{Mapper[detailTypeName.ToLower()]}>";
                }
                else
                {
                    res = $"Array<{detailTypeName}>";
                }

            }
            else
            {
                var returnNameSpace = targetType.Namespace ?? "";
                if (returnNameSpace.Equals("System", StringComparison.OrdinalIgnoreCase))
                {
                    var detailTypeName = targetType.Name.ToLower();
                    if (Mapper.ContainsKey(detailTypeName))
                    {
                        res = Mapper[detailTypeName];
                    }
                    else
                    {
                        res = detailTypeName.ToLower();
                    }
                }
                else
                {
                    res = $"{targetType.Name}";
                }
            }

        }
    }
}
