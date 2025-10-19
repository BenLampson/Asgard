using Asgard.Abstract;
using Asgard.Abstract.DataBase;
using Asgard.Abstract.Logger;
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
        }

        public override void SystemTryShutDown()
        {
        }
    }
}
