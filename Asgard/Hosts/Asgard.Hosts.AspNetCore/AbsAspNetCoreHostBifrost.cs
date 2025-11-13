using Asgard.Abstract.DataBase;
using Asgard.Abstract.Plugin;

namespace Asgard.Hosts.AspNetCore
{
    /// <summary>
    /// ASP.NET Core宿主的世界之树
    /// </summary>
    public abstract class AbsAspNetCoreHostBifrost(AbsDataBaseManager dbInstance, AbsLoggerProvider loggerProvider) : AbsBifrost(dbInstance, loggerProvider)
    {
        /// <summary>
        /// 当系统正在BuildApp时
        /// </summary>
        /// <param name="builder"></param>
        public abstract void OnBuildWebApp(IApplicationBuilder builder);

        /// <summary>
        /// 当DI服务初始化时
        /// </summary>
        /// <param name="service"></param>
        public abstract void OnServiceInit(IServiceCollection service);
    }
}
