using Asgard.Abstract.Models.AsgardConfig.ChildConfigModel.PluginConfig;

namespace Asgard.Hosts.AspNetCore.FreeSql
{
    /// <summary>
    /// 一个内阁系统的管理器,你可以利用该系统快速启动项目
    /// 注意,为了保持系统的一致性,就算是配置中心自己,本质也是自己启动了自己
    /// </summary>
    public partial class HostManager
    {
        /// <summary>
        /// 从配置中读取插件
        /// </summary> 
        private void LoadPluginFromConfig()
        {
            if (NodeConfig is null || PluginManager is null || DB is null || LoggerProvider is null)
            {
                return;
            }
            foreach (var item in NodeConfig.Value.Plugins)
            {
                _ = PluginManager.LoadPlugin(DB, LoggerProvider, item);
            }
        }

        /// <summary>
        /// 加载本地目录的插件
        /// </summary>
        private void LoadPluginFromFolder()
        {
            if (NodeConfig is null || PluginManager is null || DB is null || LoggerProvider is null)
            {
                return;
            }
            var folder = Path.Combine(AppContext.BaseDirectory, NodeConfig.Value.PluginsFolder);
            if (!Directory.Exists(folder))
            {
                return;
            }
            var allPluginFolder = Directory.GetDirectories(folder);
            foreach (var item in allPluginFolder)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                var pluginName = new DirectoryInfo(item).Name;
                var pluginFile = Path.Combine(folder, pluginName, $"{pluginName}.dll");
                if (File.Exists(pluginFile))
                {
                    _ = PluginManager.LoadPlugin(DB, LoggerProvider, new PluginItem()
                    {
                        EntranceTypeDesc = $"{pluginName}.SSSAccessPoint",
                        FilePath = pluginFile,
                        Name = pluginName,
                    });
                }

            }
        }

        /// <summary>
        /// 把当前项目作为插件加载
        /// </summary>
        private void LoadSelfAsAPlugin()
        {
            if (NodeConfig is null || PluginManager is null || DB is null || LoggerProvider is null)
            {
                return;
            }
            if (NodeConfig.Value.SelfAsAPlugin)
            {
                NodeConfig.Value.SelfPluginInfo.FilePath = Path.Combine(AppContext.BaseDirectory, NodeConfig.Value.SelfPluginInfo.FilePath);
                _ = PluginManager.LoadPlugin(DB, LoggerProvider, NodeConfig.Value.SelfPluginInfo);
            }
        }
    }
}
