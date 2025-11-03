using System.Reflection;
using System.Xml.Linq;

using Asgard.Extends.AspNetCore.ApiModels;

namespace Asgard.Extends.AspNetCore.JSCreator
{
    /// <summary>
    /// C#的类型扩展函数
    /// </summary>
    public static class CSharpTypeExtends
    {
        /// <summary>
        /// 增加一个返回值的类型
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <param name="doc"></param>
        public static void AddNewType(this List<ControllerModelTypeInfo> list, Type target, XDocument doc)
        {
            if (list.Any(item => item.RawType is null || item.RawType.FullName == target.FullName)) //如果有,直接忽略
            {
                return;
            }
            var tmp = new ControllerModelTypeInfo()
            {
                Notic = doc.GetClassNotice(target),
                RawType = target,
                IsPageResult = target.BaseType == typeof(PageRequestBase),
                //IsResult = true,
                IsEnum = target.IsEnum
            };
            list.Add(tmp);
            if (target.IsEnum)
            {
                list.HandleEnumType(target, doc, tmp);
            }
            else
            {
                list.HandleProperties(target, doc, tmp);
            }


        }

        /// <summary>
        /// 处理枚举类型
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <param name="doc"></param>
        /// <param name="sourceType"></param>
        private static void HandleEnumType(this List<ControllerModelTypeInfo> _, Type target, XDocument __, ControllerModelTypeInfo sourceType)
        {
            foreach (var item in Enum.GetNames(target))
            {
                sourceType.Properties.Add((item, ((int)Enum.Parse(target, item)).ToString(), "", false, false, true));
            }
        }


        /// <summary>
        /// 处理字段
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="list"></param>
        /// <param name="doc"></param>
        private static void HandleProperties(this List<ControllerModelTypeInfo> list, Type target, XDocument doc, ControllerModelTypeInfo sourceType)
        {
            var allPropertyTypes = target.GetProperties();
            foreach (var pItem in allPropertyTypes)
            {
                if (sourceType.IsPageResult)
                {
                    if (pItem.Name.Equals("pageIndex", StringComparison.OrdinalIgnoreCase)
                        ||
                        pItem.Name.Equals("pageSize", StringComparison.OrdinalIgnoreCase)
                        ||
                        pItem.Name.Equals("startID", StringComparison.OrdinalIgnoreCase)
                        )
                    {
                        continue;
                    }
                }
                var item = pItem.PropertyType;
                var targetType = item;

                if (item.IsArrayOrList())
                {
                    if (item.GenericTypeArguments.Length == 0)
                    {
                        targetType = item.GetElementType();
                    }
                    else
                    {
                        targetType = item.GenericTypeArguments[0];
                    }
                }
                if (targetType?.Namespace is null)
                {
                    continue;
                }

                if (targetType.Namespace.StartsWith("System", StringComparison.OrdinalIgnoreCase))//如果是系统的不管
                {
                    TypescriptAndCSharpTypeMap.GetTypeString(targetType, "", out var res, out var isArray);
                    res = res.ToFirstLowCode();
                    var pinfo = (pItem.Name.ToFirstLowCode(), res, GetPropertySummary(pItem, doc), targetType.IsNullableType() ||
                       pItem.GetCustomAttribute<System.Runtime.CompilerServices.NullableAttribute>() != null, item.IsArrayOrList(), false);
                    sourceType.Properties.Add(pinfo);
                    continue;
                }
                else if (item.IsArrayOrList())
                {
                    TypescriptAndCSharpTypeMap.GetTypeString(targetType, "", out var res, out var isArray);

                    var pinfo = (pItem.Name.ToFirstLowCode(), res, GetPropertySummary(pItem, doc), targetType.IsNullableType(), item.IsArrayOrList(), false);
                    sourceType.Properties.Add(pinfo);
                }
                else
                {
                    sourceType.Properties.Add((pItem.Name.ToFirstLowCode(), item.Name, GetPropertySummary(pItem, doc), targetType.IsNullableType(), item.IsArrayOrList(), targetType.IsEnum));
                }



                if (list.Any(lItem => lItem.RawType is null || lItem.RawType.FullName == targetType.FullName)) //如果有,直接忽略
                {
                    continue;
                }
                list.AddNewType(targetType, doc);
            }
        }



        /// <summary>
        /// 找Class的注释
        /// </summary>
        /// <param name="method"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public static string GetClassNotice(this XDocument noticXml, Type targetType)
        {

            // 构造类的签名
            string classSignature = $"T:{targetType?.FullName?.Replace('+', '.') ?? ""}";
            // 查询对应类的注释
            var classMember = noticXml.Descendants("member")
                                 .FirstOrDefault(e => e.Attribute("name")?.Value == classSignature);

            // 查询对应方法的注释

            return classMember?.Element("summary")?.Value.Trim() ?? "未找到注释";

        }

        /// <summary>
        /// 获取类型的完整名称（支持嵌套类型）。
        /// </summary>
        private static string GetFullTypeName(Type type)
        {
            if (type.DeclaringType == null)
            {
                // 非嵌套类型，直接返回 FullName
                return type.FullName?.Replace('+', '.') ?? string.Empty;
            }

            // 嵌套类型，递归构建完整名称
            return GetFullTypeName(type.DeclaringType) + "+" + type.Name;
        }
        /// <summary>
        /// 获取指定属性的注释。
        /// </summary>
        /// <param name="property">目标属性</param>
        /// <param name="xmlFilePath">XML 注释文件路径</param>
        /// <returns>属性的注释内容</returns>
        public static string GetPropertySummary(PropertyInfo property, XDocument doc)
        {
            if (property.DeclaringType is null)
            {
                return string.Empty;
            }
            // 构造属性的签名
            string propertySignature = $"P:{GetFullTypeName(property.DeclaringType)}.{property.Name}".Replace("+", ".");

            // 查询对应属性的注释
            var member = doc.Descendants("member")
                            .FirstOrDefault(e => e.Attribute("name")?.Value == propertySignature);

            if (member != null)
            {
                // 提取 <summary> 内容
                var summaryElement = member.Element("summary");
                return summaryElement != null ? summaryElement.Value.Trim() : "No summary available";
            }

            return "没有注释.";
        }

        /// <summary>
        /// 注释,控制器操作的
        /// </summary> 
        /// <returns></returns>
        public static string GetMethodNotice(this XDocument noticXml, MethodInfo targetType)
        {
            if (targetType.DeclaringType is null)
            {
                return "未找到注释";
            }
            // 构造完整的方法签名
            string methodName = $"M:{targetType.DeclaringType.FullName}.{targetType.Name}";
            if (targetType.GetParameters().Length > 0)
            {
                string parameters = string.Join(",", targetType.GetParameters()
                                                               .Select(p => p.ParameterType.FullName));
                methodName += $"({parameters})";
            }

            // 查询对应方法的注释
            return noticXml.Descendants("member")
                                  .FirstOrDefault(e => e.Attribute("name")?.Value == methodName)?.Element("summary")?.Value.Trim() ?? "未找到注释";

        }


        public static string ToFirstLowCode(this string instance)
        {
            var res = "";
            bool convertToLow = true;
            foreach (var item in instance)
            {
                if (convertToLow)
                {
                    if (char.IsUpper(item))
                    {
                        res += char.ToLower(item);
                    }
                    else if (char.IsLower(item))
                    {
                        convertToLow = false;
                        res += item;
                    }
                }
                else
                {
                    res += item;
                }
            }
            return res;
        }
    }
}
