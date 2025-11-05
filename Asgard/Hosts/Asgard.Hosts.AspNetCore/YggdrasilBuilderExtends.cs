using Asgard.Plugin;

namespace Asgard.Hosts.AspNetCore
{
    /// <summary>
    /// 世界之树建造者扩展
    /// </summary>
    public static class YggdrasilBuilderExtends
    {
        /// <summary>
        /// 构建Asp.net Core主机
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="onWebAppbuild"></param>
        /// <returns></returns>
        public static Yggdrasil BuildAspNetCoreHost(this YggdrasilBuilder builder)
        {
            var loggerProvider = builder.LoggerProvider != null ? builder.LoggerProvider(builder.NodeConfig!) : null;
            var cacheManager = builder.CacheManager != null ? builder.CacheManager(builder.LoggerProvider != null ? builder.LoggerProvider(builder.NodeConfig!) : null, builder.NodeConfig!) : null;
            var dBManager = builder.DBManager != null ? builder.DBManager(builder.LoggerProvider != null ? builder.LoggerProvider(builder.NodeConfig!) : null, builder.NodeConfig!) : null;
            var mQ = builder.MQ;
            var authManager = builder.AuthProvider != null ? builder.AuthProvider(builder.LoggerProvider != null ? builder.LoggerProvider(builder.NodeConfig!) : null, builder.NodeConfig!) : null;
            var pluginManager = new PluginLoaderManager(loggerProvider);
            var yggdrasil = new Yggdrasil()
            {
                NodeConfig = builder.NodeConfig,
                AuthManager = authManager,
                CacheManager = cacheManager,
                DBManager = dBManager,
                LoggerProvider = loggerProvider,
                MQ = mQ,
                PluginManager = pluginManager,
            };
            return yggdrasil;
        }
    }
}
