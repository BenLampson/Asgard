using System.Runtime.InteropServices;

namespace Asgard.Hosts.AspNetCore
{
    public partial class Yggdrasil : AbsYggdrasil
    {
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
        public override async Task StartAsync()
        {
            if (NodeConfig is null)
            {
                throw new Exception("请先初始化设置");
            }
            InitAspNet();

            SystemStatusPrinter();

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

        /// <summary>
        /// 打印系统状态
        /// </summary>
        private void SystemStatusPrinter()
        {
            if (NodeConfig is null)
            {
                return;
            }
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"当前连接的配置中心:{NodeConfig.ConfigCenter.ConfigCenter}:{NodeConfig.ConfigCenter.ConfigCenterPort}");
            Console.WriteLine($"当前点位名称:{NodeConfig.Name}");
            Console.WriteLine($"当前默认数据库类型:{NodeConfig.DefaultDB.DbType} 地址:{NodeConfig.DefaultDB.DbAddress}");
            Console.WriteLine($"当前读库的配置为:{string.Join(" ", NodeConfig.DefaultDB.ReadDBAddress)}");
            if (!NodeConfig.JustJobServer && NodeConfig.WebAPIConfig is not null)
            {
                if (NodeConfig.WebAPIConfig.HttpsPort != 0)
                {
                    Console.WriteLine($"当前系统将会监听http的地址为 :https://*:{NodeConfig.WebAPIConfig.HttpsPort}");
                }
                if (NodeConfig.WebAPIConfig.HttpPort != 0)
                {
                    Console.WriteLine($"当前系统将会监听普通的HTTP的地址为 :http://*:{NodeConfig.WebAPIConfig.HttpPort}");
                }

                if (NodeConfig.WebAPIConfig.HttpV2Port != 0)
                {
                    Console.WriteLine($"当前用于HTTP2通信的地址为 :http://*:{NodeConfig.WebAPIConfig.HttpV2Port} 你可以使用该地址进行GRPC访问");
                }
                Console.WriteLine($"当前系统允许的跨域访问过滤为:{string.Join(";", NodeConfig.WebAPIConfig.WithOrigins)}");

            }
            Console.ResetColor();
            Console.WriteLine("\r\n\r\n");
        }


    }
}
