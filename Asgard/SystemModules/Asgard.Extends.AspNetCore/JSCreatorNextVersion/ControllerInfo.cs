using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;

using Asgard.Extends.AspNetCore.ApiModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Asgard.Extends.AspNetCore.JSCreatorNextVersion
{
    /// <summary>
    /// 控制器信息
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class ControllerInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 原始的控制器类型
        /// </summary>
        public Type? SourceControllerType { get; set; }
        /// <summary>
        /// 路由属性信息
        /// </summary>
        public RouteAttribute? RouterAttributeInfo { get; set; }
        /// <summary>
        /// 路由名称
        /// </summary>
        public string? RouteName { get; set; }
        /// <summary>
        /// 注释
        /// </summary>
        public string? Notice { get; set; }

        /// <summary>
        /// 控制器的函数对象
        /// </summary>
        public List<ControllerMethodInfo> Methods { get; set; } = new();

        /// <summary>
        /// 所有的模型信息
        /// </summary>
        public List<ControllerModelTypeInfo> AllModelTypes { get; set; } = new();


        /// <summary>
        /// 模型namespace
        /// </summary>
        public string ModelNameSpace
        {
            get
            {
                return $"{Name}Models";
            }
        }

        /// <summary>
        /// 加载这个控制器的所有函数
        /// </summary>
        public ControllerInfo LoadMethods(Type item, XDocument noticXml)
        {
            var routeAttr = item.GetCustomAttribute(typeof(RouteAttribute))! as RouteAttribute;
            var controllerRoute = routeAttr!.Template.ToLower().Replace("[controller]", item.Name.ToLower()[..item.Name.ToLower().LastIndexOf("controller")]);


            SourceControllerType = item;
            Name = item.Name;
            RouterAttributeInfo = routeAttr;
            RouteName = controllerRoute;
            Notice = noticXml.GetClassNotice(item);

            var rawMethod = SourceControllerType.GetMethods();
            var allMethodRawList = new List<MethodInfo>();
            Dictionary<string, List<MethodInfo>> allMethodGroup = new();
            foreach (var item1 in rawMethod)
            {
                if (!item1.IsPublic)
                {
                    continue;
                }
                if (!allMethodGroup.ContainsKey(item1.Name))
                {
                    allMethodGroup.Add(item1.Name, new List<MethodInfo>());
                }
                allMethodGroup[item1.Name].Add(item1);
            }

            foreach (var item1 in allMethodGroup)
            {
                if (item1.Value.Count == 1)
                {
                    allMethodRawList.Add(item1.Value.First());
                }
                else
                {
                    foreach (var mItem in item1.Value)
                    {
                        if (mItem.DeclaringType == item)
                        {
                            allMethodRawList.Add(mItem);
                        }
                    }
                }
            }


            var allMethodRes = allMethodRawList.Where(item =>
            {
                var method = item.GetCustomAttribute(typeof(HttpMethodAttribute));
                if (method is null)
                {
                    return false;
                }
                if (
                method is not HttpGetAttribute
                &&
                method is not HttpPostAttribute
                &&
                method is not HttpPutAttribute
                &&
                method is not HttpPatchAttribute
                &&
                method is not HttpDeleteAttribute
                )
                {
                    return false;

                }

                if (!item.ReturnType.IsGenericType)//这个版本不处理非泛型的
                {
                    return false;
                }

                if (item.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)//如果是异步泛型,


                )
                {
                    if (!item.ReturnType.GetGenericTypeDefinition().IsGenericType) //但是异步泛型不是泛型,也是自定义特殊,忽略掉)
                    {
                        return false;
                    }

                }
                else if (item.ReturnType.GetGenericTypeDefinition() != typeof(DataResponse<>))//如果上面的不是,那就只可能是这个系统确认的,如果也不是,那就跳
                {
                    return false;
                }

                return true;
            }).ToList();
            var allMethod = allMethodRes
                .Select(item =>
                {
                    return new ControllerMethodInfo().LoadInfo(item, noticXml, ModelNameSpace, AllModelTypes);
                })
                .ToList();
            Methods = allMethod;

            return this;
        }






    }
}
