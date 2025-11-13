using Asgard.Abstract;
using Asgard.Abstract.Models.AsgardConfig;
using Asgard.Caches.Redis;
using Asgard.DataBaseManager.FreeSql;
using Asgard.Hosts.AspNetCore;
using Asgard.Logger.FreeSqlProvider;
using Asgard.Tools;


var kv = AuthKVToolsMethod.CreateNewAesKeyAndVi();

var nodeConfig = new NodeConfig()
{
    DefaultDB = new()
    {
        DbType = (int)FreeSql.DataType.Sqlite,
        DbAddress = "Data Source=AsgardAspNetCoreFull.db;Pooling=true;Max Pool Size=20;"
    },
    JustJobServer = false,
    SelfAsAPlugin = true,
    SystemLog = new()
    {
        EnableConsole = true
    },
    SelfPluginInfo = new()
    {
        EntranceTypeDesc = "Asgard.AspNetCore.Full.Bifrost",
        Name = "Asgard.AspNetCore.Full",
        FilePath = "Asgard.AspNetCore.Full.dll"
    },
    WebAPIConfig = new()
    {

    },
    AuthConfig = new()
    {
        AesIV = kv.iv,
        AesKey = kv.key,
        Audience = "all",
        Issuer = "asd",
        JwtKey = AuthKVToolsMethod.CreateNewHMACSHA256Key()
    }
};
var aspHost = new YggdrasilBuilder(nodeConfig)
    .UseMemCache()
    .UseFreeSqlLogger()
    .UseFreeSqlDBManager()
    .BuildAspNetCoreHost();

await aspHost.LoadPluginFromAllSource().StartAsync();

