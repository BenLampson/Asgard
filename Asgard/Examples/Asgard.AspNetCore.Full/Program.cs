using Asgard.Abstract;
using Asgard.Abstract.Models.AsgardConfig;
using Asgard.Caches.Redis;
using Asgard.DataBaseManager.FreeSql;
using Asgard.Hosts.AspNetCore;
using Asgard.Logger.FreeSqlProvider;

var nodeConfig = new NodeConfig()
{
    DefaultDB = new()
    {
        DbType = (int)FreeSql.DataType.Sqlite,
        DbAddress = "Data Source=AsgardAspNetCoreFull.db;Pooling=true;Max Pool Size=20;"
    },
    JustJobServer = false,
    SelfAsAPlugin = true,
    WebAPIConfig = new()
    {

    }
};
var aspHost = new YggdrasilBuilder(nodeConfig)
    .UseMemCache()
    .UseFreeSqlLogger()
    .UseFreeSqlDBManager()
    .BuildAspNetCoreHost();

await aspHost.Start();

