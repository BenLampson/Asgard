using Asgard.Abstract.DataBase;
using Asgard.Abstract.Logger;
using Asgard.Abstract.Models.AsgardConfig;

using FreeSql;

namespace Asgard.DataBaseManager.FreeSql
{
    public class FreeSqlManager : AbsDataBaseManager<IFreeSql>
    {
        public FreeSqlManager(AbsLoggerProvider provider, NodeConfig configInfo) : base(provider, configInfo)
        {
        }

        public override IFreeSql? GetMyDB(string idKey, string connectionStr, int type, string[] readDB)
        {
            var keyValue = idKey;
            try
            {
                return _customDBInstance.GetOrAdd(keyValue, (key) =>
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
                            _logger.Trace(e.Sql);
                        };
#endif
                        return instance;
                    }
                    catch (Exception ex)
                    {
                        _logger.Critical("Try GetDBInstance failed.", exception: ex);
                        throw new Exception("DB Instance was null.");
                    }
                });
            }
            catch
            {
                _customDBInstance.TryRemove(keyValue, out var _);
                return null;
            }
        }

        protected override void InitDefault(NodeConfig configInfo)
        {
            Default = new FreeSqlBuilder()
                .UseConnectionString((DataType)configInfo.DefaultDB.DbType, configInfo.DefaultDB.DbAddress)
                .UseSlave(configInfo.DefaultDB.ReadDBAddress)
                .UseExitAutoDisposePool(false)
                .Build();
            Default.UseJsonMap();
            Default.CodeFirst.IsAutoSyncStructure = false;
            if (configInfo.SystemLog.MinLevel == 0)
            {
                Default.Aop.CurdAfter += (s, e) =>
                {
                    _logger.Trace(e.Sql);
                };
            }
        }
    }
}
