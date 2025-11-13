using System.Reflection;

using Asgard.Abstract;
using Asgard.Abstract.DataBase;
using Asgard.Abstract.Logger;
using Asgard.Extends.AspNetCore.JSCreator;
using Asgard.Hosts.AspNetCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Asgard.AspNetCore.Full
{
    /// <summary>
    /// 程序入口
    /// </summary>
    public class Bifrost : AbsAspNetCoreHostBifrost
    {
        /// <summary>
        /// 唯一标识字符串
        /// </summary>
        public static string x = Guid.NewGuid().ToString();

        /// <summary>
        /// 初始化 Bifrost 实例。
        /// </summary>
        /// <param name="dbInstance">数据库管理器实例</param>
        /// <param name="loggerProvider">日志提供者</param>
        public Bifrost(AbsDataBaseManager dbInstance, AbsLoggerProvider loggerProvider) : base(dbInstance, loggerProvider)
        {
        }

        /// <summary>
        /// 当系统正在 BuildApp 时的回调。
        /// </summary>
        /// <param name="builder">应用构建器</param>
        public override void OnBuildWebApp(IApplicationBuilder builder)
        {
        }

        /// <summary>
        /// 当 DI 服务初始化时的回调。
        /// </summary>
        /// <param name="service">服务集合</param>
        public override void OnServiceInit(IServiceCollection service)
        {
        }

        /// <summary>
        /// 当系统启动完成时的回调。
        /// </summary>
        /// <param name="context">Asgard 上下文</param>
        public override void OnSystemStarted(AsgardContext context)
        {
#if DEBUG
            var info = new TSApiCreator("./", "Asgard.AspNetCore.Full", Assembly.GetExecutingAssembly(), "qwe");
            info.LoadDll();
            info.CreateScript();
#endif
        }

        /// <summary>
        /// 尝试关闭系统时的回调。
        /// </summary>
        public override void SystemTryShutDown()
        {
        }
    }
}
