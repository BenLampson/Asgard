using System.Reflection;

using Asgard.Core.ContextModules.Logger.AbsInfos;

namespace Asgard.Core.ContextModules.PluginLoader
{
    /// <summary>
    /// 管理器
    /// </summary>
    public partial class PluginLoaderManager
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly AbsLogger _logger;
        /// <summary>
        /// 所有的插件实例
        /// </summary>
        public List<PluginInstance> AllPluginInstance
        {
            get => this._items.OrderBy(item =>
            {
                if (item.EnteranceInstance is null)
                {
                    return 0;
                }
                else
                {
                    return item.EnteranceInstance.Order;
                }
            }).ToList();
        }

        /// <summary>
        /// 当前依赖的抽象版本
        /// </summary>
        private readonly Version _absVersion = new("10000.0.0.0");
        /// <summary>
        /// 依赖的抽象库名称
        /// </summary>
        private readonly string _absName = "";

        /// <summary>
        /// 构造函数
        /// </summary>
        public PluginLoaderManager(AbsLoggerProvider provider)
        {
            _logger = provider.CreateLogger<PluginLoaderManager>();
            if (Assembly.GetAssembly(typeof(AbsSSSAccessPoint)) is Assembly asb)
            {
                if (asb.GetCustomAttribute(typeof(AssemblyFileVersionAttribute)) is AssemblyFileVersionAttribute asbVerInfo)
                {
                    _absVersion = new Version(asbVerInfo.Version);
                    _absName = asb.GetName().Name ?? "";
                }
            }
        }





        /// <summary>
        /// 系统启动成功,异步通知.
        /// </summary>
        /// <param name="context"></param>
        public void SystemStarted(AsgardContext context)
        {
            _logger.Trace("开始调用系统启动函数[OnSystemStarted]");
            this._items.ForEach(item =>
            {
                try
                {
                    item.EnteranceInstance?.OnSystemStarted(context);
                    _logger.Trace($"调用[{item.Name}]的系统启动函数[{nameof(item.EnteranceInstance.OnSystemStarted)}]成功", eventID: context.EventID);

                }
                catch (Exception ex)
                {
                    _logger.Error($"调用[{item.Name}]的系统启动函数[{nameof(item.EnteranceInstance.OnSystemStarted)}]失败.", exception: ex, eventID: context.EventID);
                    item.StartExceptionMessage = ex.Message;
                }

            });
        }

        /// <summary>
        /// 系统启动成功,同步通知,可以阻塞容器.
        /// </summary> 
        public void SystemStoping(AsgardContext context)
        {
            _logger.Trace("开始调用系统关闭函数[SystemTryShutDown]", eventID: context.EventID);
            this._items.ForEach(item =>
            {
                try
                {
                    item.EnteranceInstance?.SystemTryShutDown();
                }
                catch (Exception ex)
                {
                    _logger.Error($"调用[{item.Name}]的系统关闭函数[{nameof(item.EnteranceInstance.SystemTryShutDown)}]失败.", exception: ex, eventID: context.EventID);
                }
            });
            _logger.Trace("系统关闭函数[SystemTryShutDown]完成.", eventID: context.EventID);
        }
    }
}
