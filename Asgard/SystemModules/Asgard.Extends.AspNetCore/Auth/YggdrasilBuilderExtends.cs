using Asgard.Abstract;

namespace Asgard.Extends.AspNetCore.Auth
{
    /// <summary>
    /// Yggdrasil 构建器扩展
    /// </summary>
    public static class YggdrasilBuilderExtends
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static YggdrasilBuilder UseAuthModule(this YggdrasilBuilder builder)
        {
            return builder.SetAuthManager((loggerProvider, nodeConfig) => new AuthManager(nodeConfig, loggerProvider));

        }
    }
}
