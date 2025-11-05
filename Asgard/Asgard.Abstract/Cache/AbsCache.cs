using Asgard.Abstract.Logger;

namespace Asgard.Abstract.Cache
{
    /// <summary>
    /// 缓存管理对象
    /// </summary>
    public abstract class AbsCache
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly AbsLogger? _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="loggerProvider">日志提供器</param>
        public AbsCache(AbsLoggerProvider? loggerProvider)
        {
            _logger = loggerProvider?.CreateLogger<AbsCache>();
        }

        /// <summary>
        /// 根据key获取某一个缓存,自己用优先级判定
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="key">key</param> 
        /// <returns>从缓存获取的值,可能为空</returns>
        public abstract T? TryGet<T>(string key) where T : class;

        /// <summary>
        /// 根据key获取某一个缓存,自己用优先级判定
        /// </summary> 
        /// <param name="key">key</param> 
        /// <returns>从缓存获取的值,可能为空</returns>
        public abstract string? TryGetString(string key);

        /// <summary>
        /// 判断key是否存在,会延续存续时长
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract bool Exists(string key);

        /// <summary>
        /// 获取某个对象的字符串表示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract string GetInstanceString<T>(T data);

        /// <summary>
        /// 保存一个字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="settings"></param>
        public abstract void SetString(string key, string value, CacheItemSettings settings);

        /// <summary>
        /// 根据某一个匹配模型获取所有key
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public abstract string[] GetKeys(string pattern);

        /// <summary>
        /// 移除一个缓存
        /// </summary>
        /// <param name="key">key</param>
        public abstract void Remove(string key);

        /// <summary>
        /// 刷新一下某个缓存
        /// </summary>
        /// <param name="key">key</param> 
        public abstract void Refresh(string key);

        /// <summary>
        /// 根据Key获取,如果获取不到则新增
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="onSet">当获取不到的时候从哪获取值</param>
        /// <param name="settings">过期时间</param> 
        /// <returns></returns>
        public T GetOrSet<T>(string key, Func<T> onSet, CacheItemSettings settings) where T : class
        {
            var res = TryGet<T>(key);
            if (res is not null)
            {
                return res;
            }
            else
            {
                var data = onSet();
                Set(key, data, settings);
                return data;
            }

        }

        /// <summary>
        /// 根据Key获取,如果获取不到则新增
        /// </summary> 
        /// <param name="key">key</param>
        /// <param name="onSet">当获取不到的时候从哪获取值</param>
        /// <param name="settings">过期时间</param> 
        /// <returns></returns>
        public string GetOrSetString(string key, Func<string> onSet, CacheItemSettings settings)
        {
            var res = TryGetString(key);
            if (res is not null)
            {
                return res;
            }
            else
            {
                var data = onSet();
                Set(key, data, settings);
                return data;
            }

        }

        /// <summary>
        /// 设置一个值到缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="data">数据</param>
        /// <param name="settings">过期时间</param>
        public void Set<T>(string key, T data, CacheItemSettings settings)
        {
            try
            {
                var realStringData = "";
                if (data is string str)
                {
                    realStringData = str;
                }
                else
                {
                    realStringData = GetInstanceString(data);
                }
                SetString(key, realStringData, settings);
            }
            catch (Exception ex)
            {
                _logger?.Information($"保存缓存{key}出错!", exception: ex);
            }
        }

    }
}
