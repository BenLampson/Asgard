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
        public static string x = Guid.NewGuid().ToString();
        public Bifrost(AbsDataBaseManager dbInstance, AbsLoggerProvider loggerProvider) : base(dbInstance, loggerProvider)
        {
        }

        public override void OnBuildWebApp(IApplicationBuilder builder)
        {
        }

        public override void OnServiceInit(IServiceCollection service)
        {
        }

        public override void OnSystemStarted(AsgardContext context)
        {
#if DEBUG
            var info = new TSApiCreator("./", "Asgard.AspNetCore.Full", Assembly.GetExecutingAssembly(), "qwe");
            info.LoadDll();
            info.CreateScript();
#endif
        }

        public override void SystemTryShutDown()
        {
        }
    }
}
