using FreeSql;

using ShangShuSheng.Core.ContextModules.AspNet.Auth;
using ShangShuSheng.Core.ContextModules.Cache;
using ShangShuSheng.Core.ContextModules.DB;
using ShangShuSheng.Core.ContextModules.Job;
using ShangShuSheng.Core.ContextModules.Logger;
using ShangShuSheng.Core.ContextModules.MQ.RabbitMQ;
using ShangShuSheng.Core.ContextModules.PluginLoader;
using ShangShuSheng.Core.ContextModules.Tools;

namespace Asgard.Hosts.AspNetCore
{
    /// <summary>
    /// 一个内阁系统的管理器,你可以利用该系统快速启动项目
    /// 注意,为了保持系统的一致性,就算是配置中心自己,本质也是自己启动了自己
    /// </summary>
    public partial class HostManager
    {

        /// <summary>
        /// 根据配置启动服务
        /// </summary>
        private void UseConfigStartSystem()
        {
            if (NodeConfig.Value.AuthConfig is null
               || string.IsNullOrWhiteSpace(NodeConfig.Value.AuthConfig.Audience)
               || string.IsNullOrWhiteSpace(NodeConfig.Value.AuthConfig.Issuer)
               || string.IsNullOrWhiteSpace(NodeConfig.Value.AuthConfig.JwtKey)
               || string.IsNullOrWhiteSpace(NodeConfig.Value.AuthConfig.AesIV)
               || string.IsNullOrWhiteSpace(NodeConfig.Value.AuthConfig.AesKey)
               )
            {
                throw new Exception($"默认的安全配置为空!");
            }
            if (!NodeConfig.Value.JustJobServer && NodeConfig.Value.WebAPIConfig is null)
            {
                throw new Exception($"设置了WebApi模式,但是WebApi没有配置");
            }

            LoggerProvider = new LoggerProvider(NodeConfig.Value.SystemLog);
            if (LoggerProvider is null)
            {
                throw new Exception($"系统日志初始化失败");
            }
            if (NodeConfig.Value.DefaultDB is null)
            {
                throw new Exception($"数据库配置为空");
            }
            DB = new DataBaseManager(LoggerProvider, NodeConfig);
            AuthManager = new AuthManager(NodeConfig.Value.AuthConfig.JwtKey, NodeConfig.Value.AuthConfig.Issuer, NodeConfig.Value.AuthConfig.Audience);
            try
            {
                BaseEncryptionTools.SetKeyAndIV(NodeConfig.Value.AuthConfig.AesKey, NodeConfig.Value.AuthConfig.AesIV);
            }
            catch (Exception ex)
            {
                throw new Exception($"初始化加密部分出错{ex.Message} aesKey:{NodeConfig.Value.AuthConfig.AesKey} aesIV:{NodeConfig.Value.AuthConfig.AesIV}");
            }
            JobManager = new JobManager(LoggerProvider);
            if (NodeConfig.Value.IdWorker is not null)
            {
                IDGen.Init(NodeConfig.Value.IdWorker.WorkID, NodeConfig.Value.IdWorker.DataCenterID);
            }
            else
            {
                IDGen.Init(1, 1);
            }
            if (NodeConfig.Value.MqConfig is null ||
                string.IsNullOrWhiteSpace(NodeConfig.Value.MqConfig.Password) ||
                string.IsNullOrWhiteSpace(NodeConfig.Value.MqConfig.UserName) ||
                string.IsNullOrWhiteSpace(NodeConfig.Value.MqConfig.Server)
                )
            {
                Console.Write($"MQ没有配置，跳过");
            }
            else
            {
                MQ = new RabbitMQManager(LoggerProvider, NodeConfig.Value.MqConfig);
            }
            CacheManager = new CacheManager(LoggerProvider);


            if (NodeConfig.Value.RedisConfig is null || string.IsNullOrWhiteSpace(NodeConfig.Value.RedisConfig.Address))
            {
                Console.WriteLine($"没有配置redis,跳过启动,目前只会存在本地的内存级别缓存");
            }
            else
            {
                CacheManager.PushRedis(NodeConfig.Value.RedisConfig.GetConnStr(), LoggerProvider.CreateLogger("RedisCacheClient"));
            }

            if (NodeConfig.Value.MqConfig is null)
            {
                Console.WriteLine($"MQ没有配置,跳过MQ初始化");
            }

            PluginManager = new PluginLoaderManager(LoggerProvider);

            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"当前连接的配置中心:{NodeConfig.Value.ConfigCenter.ConfigCenter}:{NodeConfig.Value.ConfigCenter.ConfigCenterPort}");
            Console.WriteLine($"当前点位名称:{NodeConfig.Name}");
            Console.WriteLine($"当前默认数据库类型:{(Enum.GetName((DataType)NodeConfig.Value.DefaultDB.DbType))} 地址:{NodeConfig.Value.DefaultDB.DbAddress}");
            Console.WriteLine($"当前读库的配置为:{string.Join(" ", NodeConfig.Value.DefaultDB.ReadDBAddress)}");
            if (!NodeConfig.Value.JustJobServer && NodeConfig.Value.WebAPIConfig is not null)
            {
                if (NodeConfig.Value.WebAPIConfig.HttpsPort != 0)
                {
                    Console.WriteLine($"当前系统将会监听http的地址为 :https://*:{NodeConfig.Value.WebAPIConfig.HttpsPort}");
                }
                if (NodeConfig.Value.WebAPIConfig.HttpPort != 0)
                {
                    Console.WriteLine($"当前系统将会监听普通的HTTP的地址为 :http://*:{NodeConfig.Value.WebAPIConfig.HttpPort}");
                }

                if (NodeConfig.Value.WebAPIConfig.HttpV2Port != 0)
                {
                    Console.WriteLine($"当前用于HTTP2通信的地址为 :http://*:{NodeConfig.Value.WebAPIConfig.HttpV2Port} 你可以使用该地址进行GRPC访问");
                }
                Console.WriteLine($"当前系统允许的跨域访问过滤为:{string.Join(";", NodeConfig.Value.WebAPIConfig.WithOrigins)}");

            }
            Console.ResetColor();
            Console.WriteLine("\r\n\r\n");
        }
    }
}
