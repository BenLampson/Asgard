using System.Reflection;
using System.Text;
using System.Xml.Linq;

using Microsoft.AspNetCore.Mvc;

namespace Asgard.Extends.AspNetCore.JSCreatorNextVersion
{
    public class NextVersionJSCreator
    {
        /// <summary>
        /// 要创建的文件夹
        /// </summary>
        private readonly string _folder;
        /// <summary>
        /// 入口名称
        /// </summary>
        private readonly string _entranceName;
        /// <summary>
        /// 程序集
        /// </summary>
        private readonly Assembly _ass;
        /// <summary>
        /// 程序集名称
        /// </summary>
        private readonly string _assName;
        private readonly string _apiKey;
        /// <summary>
        /// 注释XML
        /// </summary>
        private readonly XDocument _noticXml;
        /// <summary>
        /// 所有的API用于生成ReadMe的markdown文件
        /// </summary>
        private readonly Dictionary<string, List<string>> _allApis = new();
        /// <summary>
        /// 临时的代码信息
        /// </summary>
        private Dictionary<string, TempCodeInfo> TempCodeInfos { get; set; } = new();

        /// <summary>
        /// 所有的控制器
        /// </summary>
        private List<ControllerInfo> AllController { get; set; } = new();


        public NextVersionJSCreator(string folder, string entranceName, Assembly ass, string resultFolder)
        {
            _folder = resultFolder;
            _entranceName = entranceName;
            _ass = ass;
            _assName = _ass.GetName().Name!;
            _apiKey = _assName[(_assName.LastIndexOf(".") + 1)..];
            _noticXml = new XDocument();
            if (File.Exists(Path.Combine(folder, $"{_entranceName}.xml")))
            {
                _noticXml = XDocument.Parse(File.ReadAllText(Path.Combine(folder, $"{_entranceName}.xml")));
            }
        }

        /// <summary>
        /// 生成
        /// </summary>
        public void LoadDll()
        {
            AllController = _ass.GetExportedTypes().Where(item =>
            {
                if (!item.Name.EndsWith("Controller"))//必须是controller结尾
                {
                    return false;
                }
                if (item.GetCustomAttribute(typeof(ApiControllerAttribute)) is null)//必须是个API
                {
                    return false;
                }

                if (item.GetCustomAttribute(typeof(RouteAttribute)) == null)//必须有路由信息
                {
                    return false;
                }
                return true;

            }).Select(item =>
            {
                return new ControllerInfo().LoadMethods(item, _noticXml);
            }).ToList();//检索到所有的控制器 
        }

        /// <summary>
        /// 构造TS脚本
        /// </summary>
        public void CreateScript()
        {

            foreach (var controllerItem in AllController)
            {
                var apiInfosMD = new List<string>();

                var controllerFileSB = new StringBuilder();
                _ = controllerFileSB.AppendLine()
                    .AppendLine("import { ConstructQueryString, DataResponse, HttpClientPoolInstance, PageDataResponse, PageRequestBase } from 'asgard-fe-core';")
                    .AppendLine()
                    .AppendLine($"/** {controllerItem.Notice} */")
                    .AppendLine($"export namespace {controllerItem.Name}Module {{")
                    ;
                //模型的处理
                {
                    foreach (var modelItem in controllerItem.AllModelTypes)
                    {
                        if (modelItem.RawType is null)
                        {
                            continue;
                        }
                        _ = controllerFileSB.AppendLine($"\t/** {modelItem.Notic} */");
                        if (modelItem.IsPageResult)
                        {
                            _ = controllerFileSB.AppendLine($"\texport interface {modelItem.RawType.Name} extends PageRequestBase {{");
                        }
                        else
                        {
                            if (modelItem.IsEnum)
                            {
                                _ = controllerFileSB.AppendLine($"\texport enum {modelItem.RawType.Name} {{");
                            }
                            else
                            {
                                _ = controllerFileSB.AppendLine($"\texport interface {modelItem.RawType.Name} {{");
                            }
                        }

                        foreach (var pItem in modelItem.Properties)
                        {
                            if (modelItem.IsEnum)
                            {
                                _ = controllerFileSB.AppendLine($"\t\t{pItem.name} = {pItem.type}, ");
                            }

                            else
                            {
                                _ = controllerFileSB.AppendLine($"\t\t/**{pItem.description} */");
                                _ = controllerFileSB.Append($"\t\t{pItem.name}{(pItem.nullAble ? " ? " : "")}: ");
                                //_ = controllerFileSB.Append($"\t\t{pItem.name}?: "); //暂定全部可空
                                if (pItem.isArray)
                                {
                                    _ = controllerFileSB.AppendLine($"Array<{pItem.type}>");
                                }
                                else
                                {
                                    _ = controllerFileSB.AppendLine($"{pItem.type}");
                                }
                            }
                        }
                        _ = controllerFileSB.AppendLine($"\t}}");
                    }

                }


                {

                    _ = controllerFileSB.AppendLine($"\tconst controllerPrefix = '{controllerItem.RouteName}';")
                    .AppendLine($"\tconst apiKey = '{_assName}';")
                    ;
                    foreach (var mItem in controllerItem.Methods)//遍历所有函数
                    {
                        if (mItem.SourceMethodTypeInfo is null)
                        {
                            continue;
                        }
                        apiInfosMD.Add($"1. {_apiKey}.{controllerItem.Name.Replace("Controller", "")}.Controller.{mItem.RouteName} : {mItem.Notice}");

                        _ = controllerFileSB.AppendLine($"\t/** {mItem.Notice} **/")
                            .Append($"\texport const {mItem.SourceMethodTypeInfo.Name} = async (")
                            ;

                        _ = controllerFileSB.Append(string.Join(", ", mItem.Parameters.Select(pItem => $"{pItem.name}: {pItem.type}")))
                            .Append(mItem.Parameters.Count != 0 ? "," : "")
                            .Append($"head?: Record<string, string | number | boolean>,uploadProgress?: (event: any) => void,downloadProgress?: (event: any) => void, signal?: AbortSignal): ");
                        if (mItem.ResultTypeIsArray)//如果是列表,假装都是分页的
                        {
                            _ = controllerFileSB.AppendLine($"Promise<PageDataResponse<{mItem.ResultType}>> => {{");
                        }
                        else
                        {
                            _ = controllerFileSB.AppendLine($"Promise<DataResponse<{mItem.ResultType}>> => {{");
                        }

                        if (!string.IsNullOrWhiteSpace(mItem.IFromFilePatameter))//如果有文件
                        {
                            _ = controllerFileSB.AppendLine("\t\tlet data = new FormData();")
                                .AppendLine($"\t\tif ( {mItem.IFromFilePatameter} != null) {{")
                                .AppendLine($"\t\t\tdata.append('{mItem.IFromFilePatameter}', {mItem.IFromFilePatameter});")
                                .AppendLine("\t\t}")
                                ;
                        }

                        //在这里处理请求发送的问题
                        _ = controllerFileSB.Append($"\t\tlet res = await HttpClientPoolInstance.getInstance(apiKey).");
                        var method = mItem.HttpMethod switch
                        {
                            HttpGetAttribute => "get<",
                            HttpPostAttribute => "post<",
                            HttpPutAttribute => "put<",
                            HttpPatchAttribute => "patch<",
                            HttpDeleteAttribute => "delete<",
                            _ => ""
                        };
                        _ = controllerFileSB.Append(method)
                            .Append($"{mItem.ResultType}>(")
                            ;

                        if (!string.IsNullOrWhiteSpace(mItem.QueryParameter))//从query来的参数不是空
                        {
                            _ = controllerFileSB.Append($"ConstructQueryString(");
                        }
                        _ = controllerFileSB.Append($"`{"$"}{{controllerPrefix}}/{mItem.RouteName.Replace("{", "${")}`");

                        if (!string.IsNullOrWhiteSpace(mItem.QueryParameter))
                        {
                            _ = controllerFileSB.Append($", {mItem.QueryParameter})");
                        }


                        if (!string.IsNullOrWhiteSpace(mItem.IFromFilePatameter))//从body来的参数
                        {
                            _ = controllerFileSB.Append($", data");
                        }
                        else if (!string.IsNullOrWhiteSpace(mItem.BodyParameter))
                        {
                            _ = controllerFileSB.Append($",  {mItem.BodyParameter}");
                        }
                        else if (mItem.HttpMethod is not HttpGetAttribute && mItem.HttpMethod is not HttpDeleteAttribute)
                        {
                            _ = controllerFileSB.Append($", undefined");
                        }


                        _ = controllerFileSB.AppendLine($",head,uploadProgress,downloadProgress, signal);");

                        _ = controllerFileSB.AppendLine("\t\treturn res;");


                        _ = controllerFileSB.AppendLine("\t}").AppendLine("");

                    }
                }
                _ = controllerFileSB.AppendLine("}");


                TempCodeInfos.Add(controllerItem.Name, new TempCodeInfo()
                {
                    AllCode = controllerFileSB.ToString(),
                });
                _allApis.Add(controllerItem.Name, apiInfosMD);
            }
            _ = new ProjectCreator(_folder, TempCodeInfos, _ass, _allApis).Create();

        }


    }
}
