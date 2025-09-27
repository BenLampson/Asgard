using Asgard.Plugin;

namespace Asgard.Hosts.AspNetCore
{
    public partial class Yggdrasil : AbsYggdrasil
    {
        /// <summary>
        /// 插件加载管理器
        /// </summary>
        public PluginLoaderManager? PluginManager { get; set; }
        /// <summary>
        /// Asp.net app
        /// </summary>
        public WebApplication? WebApp { get; private set; }

        /// <summary>
        /// 在构建WebApplicationBuilder时的回调
        /// </summary>
        public Action<WebApplicationBuilder>? OnWebAppbuild { get; init; }



        public Yggdrasil()
        {

        }


        /// <summary>
        /// 构建世界之树对象
        /// </summary>
        public Yggdrasil Build()
        {
            return this;
        }
    }
}
