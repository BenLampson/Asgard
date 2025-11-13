using Asgard.Abstract.DataBase;
using Asgard.Abstract.Logger;
using Asgard.Abstract.Models.AsgardConfig;

using FreeSql;

namespace Asgard.DataBaseManager.FreeSql
{
    /// <summary>
    /// FreeSql 数据库管理器，提供数据库实例管理和初始化功能。
    /// </summary>
    public class FreeSqlManager : AbsDataBaseManager
    {
        /// <summary>
        /// 初始化 FreeSqlManager 实例。
        /// </summary>
        /// <param name="provider">日志提供者</param>
        /// <param name="configInfo">节点配置</param>
        public FreeSqlManager(AbsLoggerProvider? provider, NodeConfig configInfo) : base(provider, configInfo)
        {
        }

        /// <summary>
        /// 获取指定数据库实例。
        /// </summary>
        /// <typeparam name="ORMType">ORM 类型</typeparam>
        /// <param name="idKey">实例标识</param>
        /// <param name="connectionStr">连接字符串</param>
        /// <param name="type">数据库类型</param>
        /// <param name="readDB">只读数据库地址数组</param>
        /// <returns>数据库实例</returns>
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

        /// <summary>
        /// 初始化默认数据库实例。
        /// </summary>
        /// <param name="configInfo">节点配置</param>
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
