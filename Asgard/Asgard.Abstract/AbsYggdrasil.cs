using Asgard.Abstract.Auth;
using Asgard.Abstract.Cache;
using Asgard.Abstract.DataBase;
using Asgard.Abstract.Job;
using Asgard.Abstract.Logger;
using Asgard.Abstract.Models.AsgardConfig;
using Asgard.Abstract.Models.AsgardConfig.ChildConfigModel.PluginConfig;
using Asgard.Abstract.MQ;
using Asgard.Plugin;

namespace Asgard.Abstract
{
    /// <summary>
    /// 抽象级别的世界之树
    /// </summary>
    public abstract class AbsYggdrasil
    {
        /// <summary>
        /// 插件加载管理器
        /// </summary>
        public PluginLoaderManager? PluginManager { get; set; }
        /// <summary>
        /// 事件ID,每次启动都会变
        /// </summary>
        public string EventID { get; init; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 当前节点配置
        /// </summary>
        public NodeConfig? NodeConfig { get; init; }
        /// <summary>
        /// 权限模块 
        /// </summary>
        public AbsAuthManager? AuthManager { get; init; }

        /// <summary>
        /// 日志提供器
        /// </summary>
        public AbsLoggerProvider? LoggerProvider { get; init; }

        /// <summary>
        /// 数据库管理器
        /// </summary>
        public AbsDataBaseManager? DBManager { get; init; }

        /// <summary>
        /// Job管理器
        /// </summary>
        public AbsJobManager? JobManager { get; init; }

        /// <summary>
        /// 本地缓存实例
        /// </summary>
        public AbsCache? CacheManager { get; init; }

        /// <summary>
        /// MQ库管理器
        /// </summary>
        public AbsMQManager? MQ { get; init; }



        /// <summary>
        /// 获取一个新的上下文对象
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public AsgardContext GetContext()
        {
            if (NodeConfig == null)
            {
                throw new ArgumentNullException("请先初始化系统配置");
            }
            var context = new AsgardContext(NodeConfig, LoggerProvider, CacheManager, DBManager, MQ, AuthManager, Guid.NewGuid().ToString("N"));
            return context;
        }


        /// <summary>
        /// 从所有来源加载插件
        /// </summary>
        public AbsYggdrasil LoadPluginFromAllSource()
        {
            LoadSelfAsAPlugin();
            LoadPluginFromFolder();
            LoadPluginFromConfig();
            return this;
        }

        /// <summary>
        /// 以异步方式启动世界之树
        /// </summary>
        /// <returns></returns>
        public abstract Task StartAsync();

        /// <summary>
        /// 从配置中读取插件
        /// </summary> 
        public void LoadPluginFromConfig()
        {
            if (NodeConfig is null || PluginManager is null)
            {
                return;
            }
            foreach (var item in NodeConfig.Plugins)
            {
                _ = PluginManager.LoadPlugin(DBManager, LoggerProvider, item);
            }
        }

        /// <summary>
        /// 加载本地目录的插件
        /// </summary>
        public void LoadPluginFromFolder()
        {
            if (NodeConfig is null || PluginManager is null)
            {
                return;
            }
            var folder = Path.Combine(AppContext.BaseDirectory, NodeConfig.PluginsFolder);
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
                    _ = PluginManager.LoadPlugin(DBManager, LoggerProvider, new PluginItem()
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
        public void LoadSelfAsAPlugin()
        {
            if (NodeConfig is null || PluginManager is null)
            {
                return;
            }
            if (NodeConfig.SelfAsAPlugin)
            {
                NodeConfig.SelfPluginInfo.FilePath = Path.Combine(AppContext.BaseDirectory, NodeConfig.SelfPluginInfo.FilePath);
                _ = PluginManager.LoadPlugin(DBManager, LoggerProvider, NodeConfig.SelfPluginInfo);
            }
        }

    }
}
