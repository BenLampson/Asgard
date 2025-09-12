using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

using Asgard.Abstract.Logger;
using Asgard.Abstract.Models.AsgardConfig;

namespace Asgard.Abstract.DataBase
{
    /// <summary>
    /// 抽象的数据库管理器
    /// </summary>
    public abstract class AbsDataBaseManager<T>
    { /// <summary>
      /// sql对象
      /// </summary>
        public T Default { get; protected set; }

        /// <summary>
        /// 日志器
        /// </summary>
        protected readonly AbsLogger _logger;

        /// <summary>
        /// 用户自定义的数据库实例
        /// </summary>
        protected readonly ConcurrentDictionary<string, T> _customDBInstance = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="configInfo"></param> 
        public AbsDataBaseManager(AbsLoggerProvider provider, NodeConfig configInfo)
        {
            _logger = provider.CreateLogger<AbsDataBaseManager<T>>();
            InitDefault(configInfo);
            if (Default is null)
            {
                throw new ValidationException("Default DB instance is null.");
            }
        }
        /// <summary>
        /// 初始化默认数据库
        /// </summary>
        /// <param name="configInfo">节点配置</param>
        protected abstract void InitDefault(NodeConfig configInfo);



        /// <summary>
        /// 获取我的数据库
        /// </summary>
        /// <param name="idKey">数据库key,在池子里要唯一,也就是当前实例唯一</param> 
        /// <param name="connectionStr">写库链接字符串</param>
        /// <param name="type">数据库类型</param>
        /// <param name="readDB">读库</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public abstract T? GetMyDB(string idKey, string connectionStr, int type, string[] readDB);


        /// <summary>
        /// 获取某个数据库,可能是空,并且不会自动连接
        /// </summary>
        /// <param name="idKey"></param>
        /// <returns></returns>
        public T? GetMyDB(string idKey)
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
                if (Default is IDisposable canDispose)
                {
                    canDispose.Dispose();
                }
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
                            if (instance is IDisposable canDispose)
                            {
                                canDispose.Dispose();
                            }
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
