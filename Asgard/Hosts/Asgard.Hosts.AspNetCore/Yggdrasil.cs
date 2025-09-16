using Asgard.Plugin;

namespace Asgard.Hosts.AspNetCore
{
    public partial class Yggdrasil<ORMType> : AbsYggdrasil
    {
        /// <summary>
        /// 插件加载管理器
        /// </summary>
        public PluginLoaderManager<ORMType>? PluginManager { get; set; }
    }
}
