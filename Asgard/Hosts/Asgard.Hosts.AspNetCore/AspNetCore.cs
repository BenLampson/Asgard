using System.Net;
using System.Text.Encodings.Web;
using System.Text.Unicode;

using Asgard.Abstract.Models.AsgardConfig;
using Asgard.Extends.AspNetCore.ApiModels;
using Asgard.Plugin;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerUI;

namespace Asgard.Hosts.AspNetCore
{
    public partial class Yggdrasil
    {
        /// <summary>
        /// 跨域配置昵称
        /// </summary>
        private static readonly string _myAllowSpecificOrigins = "_ShangShuSheng.Core.Cros";

        /// <summary>
        /// 初始化一个Asp.Net对象出来,做自宿主 
        /// </summary>
        /// <returns></returns>
        private void InitAspNet()
        {
            if (NodeConfig is null || NodeConfig.JustJobServer || NodeConfig.WebAPIConfig is null || PluginManager is null)
            {
                return;
            }
            var builder = WebApplication.CreateBuilder();
            InitHost(builder, NodeConfig.WebAPIConfig);
            InitServices(builder, PluginManager, NodeConfig.WebAPIConfig);
            InitDI(builder, LoggerProvider);
            WebApp = builder.Build();
            _ = WebApp.UsePathBase(NodeConfig.WebAPIConfig.ApiPrefix);
            InitStaticFile(WebApp, builder, NodeConfig.WebAPIConfig);
            InitSwagger(WebApp, NodeConfig.WebAPIConfig, PluginManager);
            ConfigWebsocket(WebApp);
            RoutingConfig(WebApp, PluginManager);
        }

        /// <summary>
        /// 初始化host
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="webApiConfig"></param>
        private void InitHost(WebApplicationBuilder builder, WebApiConfig webApiConfig)
        {
            _ = builder.WebHost.UseKestrel(option =>
            {
                option.Limits.MaxRequestBodySize = null;
                if (
                !string.IsNullOrWhiteSpace(webApiConfig.CertificateFile)
                &&
                !string.IsNullOrWhiteSpace(webApiConfig.CertificatePassword)
                &&
               webApiConfig.HttpsPort != 0
                )
                {
                    option.ListenAnyIP(webApiConfig.HttpsPort, config =>
                    {
                        _ = config.UseHttps(
                            new System.Security.Cryptography.X509Certificates.X509Certificate2(
                               Path.Combine(AppContext.BaseDirectory, webApiConfig.CertificateFile),
                                webApiConfig.CertificatePassword));
                    });
                }
                if (webApiConfig.HttpPort != 0)
                {
                    option.ListenAnyIP(webApiConfig.HttpPort, config =>
                    {
                        config.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
                    });
                }
                if (webApiConfig.HttpV2Port != 0)
                {
                    option.ListenAnyIP(webApiConfig.HttpV2Port, config =>
                    {
                        config.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                    });
                }
            });
            _ = builder.Logging.ClearProviders();
        }

        /// <summary>
        /// 初始化依赖服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="plugin"></param>
        /// <param name="webapiConfig"></param>
        private void InitServices(WebApplicationBuilder builder, PluginLoaderManager plugin, WebApiConfig webapiConfig)
        {
            _ = builder.Services.AddControllers()
                .ConfigureApplicationPartManager(action =>
                {
                    plugin.AllPluginInstance.Where(plugin => plugin.AllApi.Count != 0)
                    .ToList().ForEach(plugin =>
                    {
                        if (NodeConfig is not null && NodeConfig.SelfAsAPlugin)//如果自己就是插件,那就要跳过自己
                        {
                            if (NodeConfig.SelfPluginInfo.FilePath.Equals(System.Reflection.Assembly.GetEntryAssembly()?.Location, StringComparison.OrdinalIgnoreCase))
                            {
                                return;
                            }
                        }
                        if (plugin.Assembly is null)
                        {
                            return;
                        }
                        action.ApplicationParts.Add(new AssemblyPart(plugin.Assembly!.Assembly));
                    });
                }).AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                    options.JsonSerializerOptions.Converters.Add(new DateTime2StringConverter("yyyy-MM-dd HH:mm:ss"));
                    options.JsonSerializerOptions.Converters.Add(new Long2StringConverter());
                    options.JsonSerializerOptions.Converters.Add(new TimeSpan2StringConverter());
                });
            _ = builder.Services.AddEndpointsApiExplorer();
            if (webapiConfig.UseSwagger)
            {
                _ = builder.Services.AddSwaggerGen(c =>
                {

                    c.UseInlineDefinitionsForEnums();
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "bearer"
                    });
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type=ReferenceType.SecurityScheme,
                                    Id="Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                    c.SwaggerDoc("ShangShuSheng", new OpenApiInfo()
                    {
                        Title = "尚书省",
                        Description = "尚书省,系统承载器",
                    });
                    plugin.AllPluginInstance.ForEach(item =>
                    {
                        if (item.AllApi.Count == 0)
                        {
                            return;
                        }
                        c.SwaggerDoc(item.Name, new OpenApiInfo()
                        {
                            Title = item.Name,
                            Version = item.Version.ToString(),
                        });
                    });


                    c.DocInclusionPredicate((docName, apiDes) =>
                    {
                        return !string.IsNullOrWhiteSpace(apiDes.GroupName) && docName.Equals(apiDes.GroupName);
                    });


                    plugin.AllPluginInstance.ForEach(item =>
                    {
                        if (Directory.Exists(item.PluginFolderPath))
                        {
                            foreach (var fItem in Directory.GetFiles(item.PluginFolderPath, "*.xml"))
                            {
                                var xmlPath = fItem;
                                c.IncludeXmlComments(xmlPath, true);
                            }
                        }
                    });


                    foreach (var fItem in Directory.GetFiles(AppContext.BaseDirectory, "*.xml"))
                    {
                        var xmlPath = fItem;
                        c.IncludeXmlComments(xmlPath, true);
                    }

                });
            }



            _ = builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: _myAllowSpecificOrigins,
                    builder => builder
                    .WithOrigins(webapiConfig.WithOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            _ = builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

        }
        /// <summary>
        /// 初始化DI数据
        /// </summary> 
        private void InitDI(WebApplicationBuilder builder, AbsLoggerProvider? provider)
        {
            if (provider is not null)
            {
                _ = builder.Services.AddSingleton(_ => provider);
            }
            _ = builder.Services.AddScoped(_ => new Yggdrasil());
        }

        /// <summary>
        /// 初始化静态文件服务支持
        /// </summary>
        private void InitStaticFile(WebApplication app, WebApplicationBuilder builder, WebApiConfig webapiConfig)
        {
            if (webapiConfig.StaticFileConfig is not null && webapiConfig.StaticFileConfig.UseStaticFolder)
            {

                var folder = webapiConfig.StaticFileConfig.Folder;

                var requestPrefix = webapiConfig.StaticFileConfig.Prefix;

                if (!string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder))
                {
                    _ = app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, folder)),
                        ServeUnknownFileTypes = true,
                        ContentTypeProvider = new ContentTypeProvider(),
                        DefaultContentType = "application/octet-stream",
                        RequestPath = requestPrefix.Trim()
                    });
                    if (!string.IsNullOrWhiteSpace(webapiConfig.StaticFileConfig.AutoRedirect))
                    {
                        _ = app.Use((httpContext, req) =>
                        {
                            if (httpContext.Request.Path.Equals(requestPrefix, StringComparison.OrdinalIgnoreCase)
                            || httpContext.Request.Path.Equals("/")
                            )
                            {
                                httpContext.Response.Redirect($"{(string.IsNullOrWhiteSpace(webapiConfig.ApiPrefix) ? "" : $"/{webapiConfig.ApiPrefix}")}{requestPrefix}/{webapiConfig.StaticFileConfig.AutoRedirect}");
                                return Task.CompletedTask;
                            }

                            return req();
                        });
                    }

                }
            }
        }

        /// <summary>
        /// 初始化swagger支持部分
        /// </summary>
        private void InitSwagger(WebApplication app, WebApiConfig webAPIConfig, PluginLoaderManager pluginManager)
        {
            if (webAPIConfig.UseSwagger)
            {
                _ = app.UseDeveloperExceptionPage();
                _ = app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((doc, _) =>
                    {
                        doc.Servers.Add(new Microsoft.OpenApi.Models.OpenApiServer()
                        {
                            Url = webAPIConfig.SwaggerUrlPrefix
                        });
                    });
                });
                _ = app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"{webAPIConfig.SwaggerUrlPrefix}/swagger/ShangShuSheng/swagger.json", "ShangShuSheng");
                    pluginManager.AllPluginInstance.ForEach(item =>
                    {
                        if (item.AllApi.Count == 0)
                        {
                            return;
                        }
                        c.SwaggerEndpoint($"{webAPIConfig.SwaggerUrlPrefix}/swagger/{item.Name}/swagger.json", item.Name);
                    });
                    c.DocExpansion(DocExpansion.None);
                });
            }
        }

        /// <summary>
        /// 配置websocket
        /// </summary>
        /// <param name="app"></param>
        private void ConfigWebsocket(WebApplication app)
        {
            _ = app.UseWebSockets(new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(30)
            });
        }

        /// <summary>
        /// 路由配置
        /// </summary>
        /// <param name="app"></param>
        /// <param name="plugin"></param>
        private void RoutingConfig(WebApplication app, PluginLoaderManager plugin)
        {
            plugin.AllPluginInstance.ForEach(item =>
            {
                if (item.EnteranceInstance is null)
                {
                    return;
                }
                item.EnteranceInstance.OnBuildWebApp(app);
            });
            _ = app.UseRouting()
              .UseExceptionHandler(builder =>
              {
                  builder.Run(async context =>
                  {
                      context.Response.StatusCode = (int)HttpStatusCode.OK;
                      context.Response.ContentType = "application/json";
                      var httpContext = context.RequestServices.GetService<AbsYggdrasil>();
                      var exception = context.Features.Get<IExceptionHandlerFeature>();
                      var res = new DataResponse<string>
                      {
                          Code = ResponseCodeEnum.Error | ResponseCodeEnum.System
                      };
                      if (exception != null)
                      {
                          var error = $"{exception.Error.Message} 事件ID:{(httpContext is null ? "事件ID空" : httpContext.EventID)}";
                          if (NodeConfig is not null && NodeConfig.SystemLog.MinLevel == LogLevelEnum.Trace)
                          {
                              res.Data = exception.Error.StackTrace;
                          }
                          res.Msg = (httpContext is null ? "事件ID空" : httpContext.EventID);
                          LoggerProvider?.CreateLogger("API_Exception")?.Error($"请求路径:[{context.Request.Path}] 请求方式:[{context.Request.Method}] 出错!", eventID: (httpContext is null ? "事件ID空" : httpContext.EventID), exception: exception.Error);
                      }
                      var jsonText = System.Text.Json.JsonSerializer.Serialize<DataResponse<string>>(res, CommonSerializerOptions.CamelCaseChineseNameCaseInsensitive);
                      await context.Response.WriteAsync(jsonText).ConfigureAwait(false);
                  });
              })
              .UseCors(_myAllowSpecificOrigins)
              .UseAuthorization()
              .UseEndpoints(endpoints =>
              {
                  plugin.AllPluginInstance.Where(plugin => plugin.AllGrpcServices.Count != 0)
                 .ToList().ForEach(plugin =>
                 {
                     foreach (var item in plugin.AllGrpcServices)
                     {
                         var rawMethodInfo = typeof(GrpcEndpointRouteBuilderExtensions).GetMethod("MapGrpcService");
                         if (rawMethodInfo is not null)
                         {
                             var method = rawMethodInfo.MakeGenericMethod(item);
                             _ = method.Invoke(null, new[] { endpoints });
                         }
                     }
                 });
                  _ = endpoints.MapControllers();
              })
              ;
        }
    }
}
