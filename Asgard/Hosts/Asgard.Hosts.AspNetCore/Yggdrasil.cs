using System.Runtime.InteropServices;

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
        public async Task Start()
        {
            if (NodeConfig is null)
            {
                throw new Exception("请先初始化设置");
            }
            InitAspNet();

            if (WebApp is not null)//web宿主
            {
                _ = WebApp.Lifetime.ApplicationStarted.Register(() =>
                {
                    try
                    {
                        SystemStart();
                    }
                    catch (Exception ex)
                    {
                        LoggerProvider?.CreateLogger<Yggdrasil>().Critical("系统启动失败!", exception: ex, eventID: EventID);
                        _ = Task.Run(async () => { await WebApp.StopAsync(); });
                    }
                });
                _ = WebApp.Lifetime.ApplicationStopping.Register(() =>
                {
                    try
                    {
                        NotifyStoping();
                    }
                    catch (Exception ex)
                    {
                        LoggerProvider?.CreateLogger<Yggdrasil>().Critical("系统停止失败!", exception: ex, eventID: EventID);
                    }
                });
                LoggerProvider?.CreateLogger<Yggdrasil>().Critical("系统启动.", eventID: EventID);
                WebApp.Start();
            }

            if (WebApp is not null)
            {
                WebApp.WaitForShutdown();
            }
            else
            {
                using var waitForProcessShutdownStart = new ManualResetEventSlim();
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    _ = PosixSignalRegistration.Create((PosixSignal)15, (context) =>
                    {
                        context.Cancel = true;
                        waitForProcessShutdownStart.Set();
                    });
                }
                else
                {

                    _ = PosixSignalRegistration.Create(PosixSignal.SIGINT, (context) =>
                    {
                        context.Cancel = true;
                        waitForProcessShutdownStart.Set();
                    });
                }

                waitForProcessShutdownStart.Wait();
            }

            Console.WriteLine("系统关闭完成,尝试销毁资源.");
            if (WebApp is not null)
            {
                await WebApp.StopAsync();
            }
            else
            {
                NotifyStoping();
            }
            Console.WriteLine("销毁完成.");
            Console.WriteLine("系统已停止.");
            LoggerProvider?.CreateLogger<Yggdrasil>().Critical("系统已停止.", eventID: EventID);
        }


    }
}
