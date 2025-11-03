using System.Reflection;
using System.Xml.Linq;

using Asgard.Extends.AspNetCore.ApiModels;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Asgard.Extends.AspNetCore.JSCreator
{

    /// <summary>
    /// 控制器的函数信息
    /// </summary>
    public class ControllerMethodInfo
    {
        /// <summary>
        /// 原始函数类型信息
        /// </summary>
        public MethodInfo? SourceMethodTypeInfo { get; set; }

        /// <summary>
        /// 从query来构造的参数
        /// </summary>
        public string? QueryParameter { get; set; }

        /// <summary>
        /// 来自Body的参数
        /// </summary>
        public string? BodyParameter { get; set; }

        /// <summary>
        /// Put的请求参数
        /// </summary>
        public string? PutParameter { get; set; }

        /// <summary>
        /// 单文件参数
        /// </summary>
        public string? IFromFilePatameter { get; set; }

        /// <summary>
        /// 路由名称
        /// </summary>
        public string RouteName { get; set; } = string.Empty;

        /// <summary>
        /// 路由类型
        /// </summary>
        public string RouteType { get; set; } = string.Empty;

        /// <summary>
        /// 注释信息
        /// </summary>
        public string Notice { get; set; } = "未找到注释";

        /// <summary>
        /// HTTP的请求类型
        /// </summary>
        public HttpMethodAttribute? HttpMethod { get; set; }

        /// <summary>
        /// 返回值类型
        /// </summary>
        public string ResultType { get; set; } = "void";

        /// <summary>
        /// 返回的是列表
        /// </summary>
        public bool ResultTypeIsArray { get; set; }


        /// <summary>
        /// 对应的属性列表 ,key 属性跟他的类型,value
        /// </summary>
        public List<(string name, string type)> Parameters { get; set; } = new();

        /// <summary>
        /// 加载这个函数的信息
        /// </summary>
        /// <param name="method"></param>
        /// <param name="noticXml"></param>
        /// <param name="modelNamespace"></param>
        /// <param name="allModel"></param>
        /// <returns></returns>
        public ControllerMethodInfo LoadInfo(MethodInfo method, XDocument noticXml, string modelNamespace, List<ControllerModelTypeInfo> allModel)
        {

            var attr = method.GetCustomAttribute(typeof(HttpMethodAttribute));
            if (attr is not HttpMethodAttribute info)
            {
                throw new Exception("不是Http函数");
            }
            HttpMethod = info;
            SourceMethodTypeInfo = method;
            RouteType = attr switch
            {
                HttpGetAttribute => "get",
                HttpPostAttribute => "post",
                HttpPutAttribute => "put",
                HttpPatchAttribute => "patch",
                HttpDeleteAttribute => "delete",
                _ => ""
            };
            RouteName = attr switch
            {
                HttpGetAttribute get => get.Template ?? "",
                HttpPostAttribute post => post.Template ?? "",
                HttpPutAttribute put => put.Template ?? "",
                HttpPatchAttribute patch => patch.Template ?? "",
                HttpDeleteAttribute delete => delete.Template ?? "",
                _ => ""
            };
            Notice = noticXml.GetMethodNotice(method);
            HandleResultType(modelNamespace, noticXml, allModel);
            HandleParameterType(noticXml, allModel);


            return this;
        }
        /// <summary>
        /// 处理返回值
        /// </summary>
        /// <param name="modelNamespace"></param>
        /// <param name="noticXml"></param>
        /// <param name="allModel"></param> 
        private void HandleResultType(string modelNamespace, XDocument noticXml, List<ControllerModelTypeInfo> allModel)
        {
            if (SourceMethodTypeInfo is null || SourceMethodTypeInfo.ReturnType == typeof(void))
            {
                return;
            }
            Type returnType = SourceMethodTypeInfo.ReturnType;
            if (returnType is null)
            {
                return;
            }
            Type detailTypeInfo;


            if (returnType.IsGenericType && typeof(Task<>) == returnType.GetGenericTypeDefinition())//如果是task类型,找里头的
            {
                returnType = returnType.GetGenericArguments()[0];
            }
            if (typeof(DataResponse<>) != returnType.GetGenericTypeDefinition())
            {
                return;
            }

            TypescriptAndCSharpTypeMap.GetTypeString(returnType, modelNamespace, out var res, out var isArray);
            ResultType = res;
            ResultTypeIsArray = isArray;

            if (isArray)//如果返回的类型是个列表,那就要继续深挖
            {
                if (returnType.GenericTypeArguments[0].GenericTypeArguments.Length == 0)
                {
                    detailTypeInfo = returnType.GenericTypeArguments[0];
                }
                else
                {
                    detailTypeInfo = returnType.GenericTypeArguments[0].GenericTypeArguments[0];
                }
            }
            else
            {
                detailTypeInfo = returnType.GenericTypeArguments[0];
            }

            if (detailTypeInfo.Namespace is null || IsSystemType(detailTypeInfo))//!detailTypeInfo.Namespace.StartsWith(assName)
            {
                return;
            }

            allModel.AddNewType(detailTypeInfo, noticXml);



        }

        /// <summary>
        /// 处理参数
        /// </summary>
        /// <param name="modelNamespace"></param>
        /// <param name="doc"></param>
        /// <param name="allModel"></param>
        /// <param name="assName"></param>
        private void HandleParameterType(XDocument doc, List<ControllerModelTypeInfo> allModel)
        {
            if (SourceMethodTypeInfo is null)
            {
                return;
            }
            var parameters = SourceMethodTypeInfo.GetParameters();
            foreach (var pItem in parameters)//循环所有参数
            {
                if (pItem.ParameterType == typeof(IFormFile))
                {
                    IFromFilePatameter = pItem.Name;
                }
                if (pItem.GetCustomAttribute(typeof(FromQueryAttribute)) is not null)//说明有从query来得参数
                {
                    QueryParameter = pItem.Name;
                }
                else if (pItem.GetCustomAttribute<FromBodyAttribute>() is not null)//从Body来得
                {
                    BodyParameter = pItem.Name;
                }
                var name = pItem.Name;
                var typeName = pItem.ParameterType.Name;
                if (!IsSystemType(pItem.ParameterType))//如果请求是个对象,那就加进去
                {
                    allModel.AddNewType(pItem.ParameterType, doc);//处理对象类型
                    typeName = $"{typeName}";
                }

                if (TypescriptAndCSharpTypeMap.Mapper.ContainsKey(typeName.ToLower()))//如果包含,那就换掉
                {
                    typeName = TypescriptAndCSharpTypeMap.Mapper[typeName.ToLower()];
                }
                if (name is null || typeName is null)
                {
                    continue;
                }
                Parameters.Add((name, typeName));
            }
        }

        public static bool IsSystemType(Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();

            // 检查是否是构造泛型类型
            if (typeInfo.IsConstructedGenericType)
            {
                return IsSystemType(typeInfo.GetGenericTypeDefinition());
            }

            // 获取定义该类型的程序集
            Assembly definingAssembly = typeInfo.Assembly;

            // 获取所有.NET核心程序集的名称
            string[] systemAssemblies = { "System.Private.CoreLib", "mscorlib" };

            // 检查类型是否在任何一个系统程序集中定义
            foreach (var assemblyName in systemAssemblies)
            {
                if ((definingAssembly.GetName().Name ?? string.Empty).Equals(assemblyName, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
