using System.Collections.Concurrent;

using Asgard.Core.ContextModules.ConfigCenter.DBModels;
using Asgard.Core.ContextModules.Logger.AbsInfos;

using FreeSql;

namespace Asgard.Core.ContextModules.DB
{
    /// <summary>
    /// 数据库管理器的抽象
    /// </summary>
    public class DataBaseManager
    {

        /// <summary>
        /// sql对象
        /// </summary>
        public IFreeSql Default { get; protected set; }

        private readonly AbsLogger _logger;

        /// <summary>
        /// 用户自定义的数据库实例
        /// </summary>
        private readonly ConcurrentDictionary<string, IFreeSql> _customDBInstance = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="configInfo"></param> 
        public DataBaseManager(AbsLoggerProvider provider, SystemConfig configInfo)
        {
            _logger = provider.CreateLogger<DataBaseManager>();

            Default = new FreeSqlBuilder()
          .UseConnectionString((DataType)configInfo.Value.DefaultDB.DbType, configInfo.Value.DefaultDB.DbAddress)
          .UseSlave(configInfo.Value.DefaultDB.ReadDBAddress)
          .UseExitAutoDisposePool(false)
          .Build();
            Default.UseJsonMap();
            Default.CodeFirst.IsAutoSyncStructure = false;
            if (configInfo.Value.SystemLog.MinLevel == 0)
            {
                Default.Aop.CurdAfter += (s, e) =>
                {
                    _logger.Trace(e.Sql);
                };
            }
        }


        /// <summary>
        /// 获取我的数据库
        /// </summary>
        /// <param name="idKey">数据库key</param> 
        /// <param name="connectionStr">写库链接字符串</param>
        /// <param name="type">数据库类型</param>
        /// <param name="readDB">读库</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IFreeSql? GetMyDB(string idKey, string connectionStr, int type, string[] readDB)
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

        /// <summary>
        /// 获取某个数据库,可能是空,并且不会自动连接
        /// </summary>
        /// <param name="idKey"></param>
        /// <returns></returns>
        public IFreeSql? GetMyDB(string idKey)
        {
            var keyValue = idKey;
            _ = _customDBInstance.TryGetValue(keyValue, out var db);
            return db;
        }

        /// <summary>
        /// 停止数据库服务
        /// </summary>
        public void Stop()
        {
            try
            {
                Default.Dispose();
            }
            catch
            {
            }
            try
            {
                var allKeys = _customDBInstance.Keys;
                foreach (var item in allKeys)
                {
                    if (_customDBInstance.TryRemove(item, out var instance))
                    {
                        try
                        {
                            instance.Dispose();
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }
        }


    }
}
