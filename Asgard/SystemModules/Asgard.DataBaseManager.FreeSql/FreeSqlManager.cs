using Asgard.Abstract.DataBase;
using Asgard.Abstract.Logger;
using Asgard.Abstract.Models.AsgardConfig;

using FreeSql;

namespace Asgard.DataBaseManager.FreeSql
{
    public class FreeSqlManager : AbsDataBaseManager
    {
        public FreeSqlManager(AbsLoggerProvider? provider, NodeConfig configInfo) : base(provider, configInfo)
        {
        }



        public override ORMType? GetMyDB<ORMType>(string idKey, string connectionStr, int type, string[] readDB) where ORMType : default
        {
            var keyValue = idKey;
            try
            {
                var target = _customDBInstance.GetOrAdd(keyValue, (key) =>
                {
                    try
                    {
                        var builder = new FreeSqlBuilder().UseConnectionString((DataType)type, connectionStr).UseExitAutoDisposePool(false);

                        builder = builder.UseSlave(readDB);
                        var instance = builder.Build();
                        instance.UseJsonMap();
                        instance.CodeFirst.IsAutoSyncStructure = false;
#if DEBUG
                        instance.Aop.CurdAfter += (s, e) =>
                        {
                            _logger?.Trace(e.Sql);
                        };
#endif
                        return instance;
                    }
                    catch (Exception ex)
                    {
                        _logger?.Critical("Try GetDBInstance failed.", exception: ex);
                        throw new Exception("DB Instance was null.");
                    }
                });
                if (target is ORMType targetInstance)
                {
                    return targetInstance;
                }
            }
            catch
            {
                _ = _customDBInstance.TryRemove(keyValue, out var _);
            }

            return default;
        }

        protected override void InitDefault(NodeConfig configInfo)
        {
            var defaultDB = new FreeSqlBuilder()
                  .UseConnectionString((DataType)configInfo.DefaultDB.DbType, configInfo.DefaultDB.DbAddress)
                  .UseSlave(configInfo.DefaultDB.ReadDBAddress)
                  .UseExitAutoDisposePool(false)
                  .Build();
            defaultDB.UseJsonMap();
            defaultDB.CodeFirst.IsAutoSyncStructure = false;
            if (configInfo.SystemLog.MinLevel == 0)
            {
                defaultDB.Aop.CurdAfter += (s, e) =>
                {
                    _logger?.Trace(e.Sql);
                };
            }
            _ = _customDBInstance.TryAdd("default", defaultDB);
        }
    }
}
