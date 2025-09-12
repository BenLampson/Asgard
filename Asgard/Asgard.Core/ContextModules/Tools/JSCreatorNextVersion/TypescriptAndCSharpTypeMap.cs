namespace Asgard.Core.ContextModules.Tools.JSCreatorNextVersion
{
    /// <summary>
    /// 映射TS与C#的类型
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

        public static void GetTypeString(Type rawTargetType, string modelNamespace, out string res, out bool isArray)
        {
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
