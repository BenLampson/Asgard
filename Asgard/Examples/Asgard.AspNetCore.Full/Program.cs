using Asgard.Abstract;
using Asgard.Caches.Redis;
using Asgard.ConfigCenter.HostExtendsModules;
using Asgard.DataBaseManager.FreeSql;
using Asgard.Hosts.AspNetCore;
using Asgard.Logger.FreeSqlProvider;

var aspHost = new YggdrasilBuilder()
    .UseNodeConfigFromFile("")
    .UseMemCache()
    .UseFreeSqlLogger()
    .UseFreeSqlDBManager()
    .BuildAspNetCoreHost();

aspHost.Build

