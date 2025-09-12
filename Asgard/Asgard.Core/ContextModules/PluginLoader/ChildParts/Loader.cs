using Asgard.Core.ContextModules.DB;
using Asgard.Core.ContextModules.Logger.AbsInfos;

namespace Asgard.Core.ContextModules.PluginLoader
{
    /// <summary>
    /// 管理器
    /// </summary>
    public partial class PluginLoaderManager
    {

        private readonly List<PluginInstance> _items = new();
        /// <summary>
        /// 
        /// </summary> 
        /// <returns></returns>
        public bool LoadPlugin(DataBaseManager dbInstance, AbsLoggerProvider loggerProvider, PluginItem plugin)
        {

            var pluginInstance = new PluginInstance(plugin);
            if (!pluginInstance.CheckPathInfo())
            {
                _logger.Warning($"文件[{pluginInstance.PluginFilePath}]不存在, 创建:{plugin.Name}加载失败.");
                return false;
            }
            try
            {
                if (!pluginInstance.TryCreateEntrance(dbInstance, loggerProvider, out var entranceInstance) || entranceInstance is null)
                {
                    _logger.Warning($"插件[{plugin.Name}]的入口类型:{plugin.EntranceTypeDesc}创建失败.");
                    return false;
                }
                var absVersion = pluginInstance.GetReferencedLibVersion(this._absName);
                if (absVersion is null)
                {
                    _logger.Warning($"插件[{plugin.Name}]没有入口.");
                    return false;
                }
                if (absVersion.Major != this._absVersion.Major)
                {
                    _logger.Warning($"跳过加载插件[{plugin.Name}],它的容器主版本是:{absVersion.Major}, 但是你使用的容器主版本是:{this._absVersion.Major}.");

                    return false;
                }
                else if (absVersion.Minor != this._absVersion.Minor)
                {
                    _logger.Warning($"跳过加载插件[{plugin.Name}], 它的容器子版本是:{absVersion.Minor},但是你的容器子版本是:{this._absVersion.Minor}.");
                    return false;
                }
                else if (this._absVersion.Revision != -1 && (absVersion.Build != this._absVersion.Build || absVersion.Revision != this._absVersion.Revision))
                {
                    _logger.Information($"插件[{plugin.Name}], 它的容器版本是:{absVersion},但是你使用的是:{this._absVersion}, 检查一下,是否需要升级.");
                }

                pluginInstance.LoadAllPlugins();
                _items.Add(pluginInstance);
                _logger.Information($"插件[{plugin.Name}]加载完成.");
            }
            catch (Exception ex)
            {
                _logger.Error($"加载插件[{plugin.Name}]出错:{ex.Message}.", exception: ex);
            }
            return true;

        }


    }
}
