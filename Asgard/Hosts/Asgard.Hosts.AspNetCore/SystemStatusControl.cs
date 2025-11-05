namespace Asgard.Hosts.AspNetCore
{
    public partial class Yggdrasil
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

            var context = GetContext();
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
                JobManager.CreateContextAction = () => GetContext();
                JobManager.Start();
            }
        }

        /// <summary>
        /// 通知关闭
        /// </summary>
        private void NotifyStoping()
        {
            var context = GetContext();
            context.EventID = EventID;
            PluginManager?.SystemStoping(context);
            JobManager?.Stop(context);
            DBManager?.Stop();
        }
    }
}
