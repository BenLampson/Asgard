using Asgard.Abstract;
using Asgard.Caches.Redis;
using Asgard.ConfigCenter.HostExtendsModules;
using Asgard.DataBaseManager.FreeSql;
using Asgard.Logger.FreeSqlProvider;

new YggdrasilBuilder()
    .UseNodeConfigFromFile("")
    .UseMemCache()
    .UseFreeSqlLogger()
    .UseFreeSqlDBManager();

