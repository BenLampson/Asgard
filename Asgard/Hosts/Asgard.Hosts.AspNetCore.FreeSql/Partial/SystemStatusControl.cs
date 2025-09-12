using ShangShuSheng.Core.ContextModules.Job;

namespace Asgard.Hosts.AspNetCore
{
    /// <summary>
    /// 一个内阁系统的管理器,你可以利用该系统快速启动项目
    /// 注意,为了保持系统的一致性,就算是配置中心自己,本质也是自己启动了自己
    /// </summary>
    public partial class HostManager
    {
        /// <summary>
        /// 启动系统内容
        /// </summary>
        private void SystemStart()
        {
            Console.WriteLine("尝试启动系统");
            NotifyCompleted();
            Console.WriteLine("尝试初始化任务器");
            InitJobServer();
            Console.WriteLine("系统启动完成!");
            LoggerProvider?.CreateLogger("SystemStart")?.Critical("系统启动完成!", eventID: EventID);
        }
        /// <summary>
        /// 完成启动后通知
        /// </summary>
        private void NotifyCompleted()
        {

            var context = CreateContext();
            context.EventID = EventID;
            PluginManager?.SystemStarted(context);
        }
        /// <summary>
        /// 初始化服务内容
        /// </summary>
        private void InitJobServer()
        {

            PluginManager?.AllPluginInstance.Where(plugin => plugin.AllJobs.Count != 0)
                .ToList().ForEach(plugin =>
                {

                    foreach (var item in plugin.AllJobs)
                    {
                        JobManager?.PushNewJobInfo(item);
                    }
                });
            if (JobManager is not null)
            {
                JobManager.CreateContextAction = () => CreateContext();
                JobManager.Start();
            }
        }

        /// <summary>
        /// 通知关闭
        /// </summary>
        private void NotifyStoping()
        {
            var context = CreateContext();
            context.EventID = EventID;
            PluginManager?.SystemStoping(context);
            JobManager?.Stop(context);
            DB?.Stop();
        }
    }
}
