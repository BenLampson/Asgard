using System.Text;
using System.Text.Json;

using Asgard.Abstract.Cache;
using Asgard.Abstract.Logger;
using Asgard.Extends.Json;

using CSRedis;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Redis;

namespace Asgard.Caches.Redis
{
    /// <summary>
    /// 缓存管理对象，支持 Redis 和内存缓存。
    /// 由于是自己实现插件,这里的正反序列化都增加了CommonSerializerOptions.CamelCaseChineseNameCaseInsensitive支持
    /// </summary>
    public class CacheManager : AbsCache
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly AbsLogger? _logger;


        /// <summary>
        /// redis客户端实例
        /// </summary>
        private CSRedisClient? _redis = null;
        /// <summary>
        /// 微软扩展的缓存实例
        /// </summary>
        private CSRedisCache? _redisCacheExtends = null;
        /// <summary>
        /// 内存缓存
        /// </summary>
        private MemoryCache MemoryCache { get; set; } = new MemoryCache(new MemoryCacheOptions()
        {
            ExpirationScanFrequency = TimeSpan.FromSeconds(5) // 设置过期检查周期为30秒
        });

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="loggerProvider">日志提供器</param>
        public CacheManager(AbsLoggerProvider? loggerProvider) : base(loggerProvider)
        {
            _logger = loggerProvider?.CreateLogger<CacheManager>();
        }

        /// <summary>
        /// 推送一个新的缓存实例
        /// </summary> 
        public void PushRedis(string redisConnString, AbsLogger? logger)
        {
            if (string.IsNullOrEmpty(redisConnString))
            {
                logger?.Warning($"Redis模块关闭,因为没有配置连接字符串.");
                return;
            }
            try
            {
                _redis = new CSRedisClient(redisConnString);
                _redisCacheExtends = new CSRedisCache(_redis);
                _redisCacheExtends.Refresh("test");

            }
            catch (Exception ex)
            {
                _redis?.Dispose();
                logger?.Critical($"Redis服务关创建失败,模块关闭", exception: ex);
            }
        }


        /// <summary>
        /// 根据key获取某一个缓存,自己用优先级判定
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="key">key</param> 
        /// <returns>从缓存获取的值,可能为空</returns>
        public override async Task<T?> TryGetAsync<T>(string key) where T : class
        {
            try
            {
                // 优先从内存获取
                var memStr = Encoding.UTF8.GetString(MemoryCache.Get<byte[]>(key) ?? []);
                if (!string.IsNullOrWhiteSpace(memStr))
                {
                    return JsonSerializer.Deserialize<T>(memStr, CommonSerializerOptions.CamelCaseChineseNameCaseInsensitive);
                }
                // 再查 Redis
                if (_redisCacheExtends is not null)
                {
                    var redisStr = await _redisCacheExtends.GetStringAsync(key).ConfigureAwait(false);
                    if (!string.IsNullOrWhiteSpace(redisStr))
                    {
                        return JsonSerializer.Deserialize<T>(redisStr, CommonSerializerOptions.CamelCaseChineseNameCaseInsensitive);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.Information($"尝试获取缓存[{key}]报错.", exception: ex);
            }
            return default;
        }




        /// <summary>
        /// 根据key获取某一个缓存,自己用优先级判定
        /// </summary> 
        /// <param name="key">key</param> 
        /// <returns>从缓存获取的值,可能为空</returns>
        public override async Task<string?> TryGetStringAsync(string key)
        {
            try
            {
                // 优先从内存获取
                var memStr = Encoding.UTF8.GetString(MemoryCache.Get<byte[]>(key) ?? []);
                if (!string.IsNullOrWhiteSpace(memStr))
                {
                    return memStr;
                }
                // 再查 Redis
                if (_redisCacheExtends is not null)
                {
                    var redisStr = await _redisCacheExtends.GetStringAsync(key).ConfigureAwait(false);
                    if (!string.IsNullOrWhiteSpace(redisStr))
                    {
                        return redisStr;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.Information($"尝试获取缓存[{key}]报错.", exception: ex);
            }
            return default;
        }

        /// <summary>
        /// 判断key是否存在,会延续存续时长
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override async Task<bool> ExistsAsync(string key)
        {
            try
            {
                // 优先查内存
                if (MemoryCache.Get<byte[]>(key) is not null)
                {
                    return true;
                }
                // 再查 Redis
                if (_redisCacheExtends is not null)
                {
                    return (await _redisCacheExtends.GetAsync(key).ConfigureAwait(false)) is not null;
                }
            }
            catch (Exception ex)
            {
                _logger?.Information($"尝试获取缓存 {key} 报错.", exception: ex);
            }
            return false;
        }






        /// <summary>
        /// 保存一个字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="settings"></param>
        public override async Task SetStringAsync(string key, string value, CacheItemSettings settings)
        {
            var bytes = Encoding.UTF8.GetBytes(value);

            if (settings.SaveToAll)
            {
                // 同时写入内存和 Redis
                var opt = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = settings.AbsoluteExpiration,
                    AbsoluteExpirationRelativeToNow = settings.AbsoluteExpirationRelativeToNow,
                    SlidingExpiration = settings.SlidingExpiration,
                };
                _ = MemoryCache.Set(key, bytes, opt);

                if (_redisCacheExtends is not null)
                {
                    var cacheSettings = new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpiration = settings.AbsoluteExpiration,
                        AbsoluteExpirationRelativeToNow = settings.AbsoluteExpirationRelativeToNow,
                        SlidingExpiration = settings.SlidingExpiration,
                    };
                    await _redisCacheExtends.SetAsync(key, bytes, cacheSettings).ConfigureAwait(false);
                }
            }
            else
            {
                if (_redisCacheExtends is not null)
                {
                    // 只写 Redis
                    var cacheSettings = new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpiration = settings.AbsoluteExpiration,
                        AbsoluteExpirationRelativeToNow = settings.AbsoluteExpirationRelativeToNow,
                        SlidingExpiration = settings.SlidingExpiration,
                    };
                    await _redisCacheExtends.SetAsync(key, bytes, cacheSettings).ConfigureAwait(false);
                }
                else
                {
                    // 只写内存
                    var opt = new MemoryCacheEntryOptions()
                    {
                        AbsoluteExpiration = settings.AbsoluteExpiration,
                        AbsoluteExpirationRelativeToNow = settings.AbsoluteExpirationRelativeToNow,
                        SlidingExpiration = settings.SlidingExpiration,
                    };
                    _ = MemoryCache.Set(key, bytes, opt);
                }
            }
        }

        /// <summary>
        /// 根据某一个匹配模型获取所有key
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public override async Task<string[]> GetKeysAsync(string pattern)
        {
            try
            {
                if (_redis is not null)
                {
                    var redisRes = await _redis.KeysAsync(pattern).ConfigureAwait(false);
                    if (redisRes is not null && redisRes.Length != 0)
                    {
                        return redisRes;
                    }
                }
                var realPattern = pattern.Replace("*", "");
                return MemoryCache.Keys.Select(item => item.ToString()).ToList().Where(item => item?.StartsWith(realPattern) ?? false)
                    .Select(item => item ?? "BadKeyData")
                    .ToArray() ?? [];
            }
            catch (Exception ex)
            {
                _logger?.Information($"尝试获取缓存Key报错. 命令:{pattern}", exception: ex);
            }
            return Array.Empty<string>();
        }

        /// <summary>
        /// 移除一个缓存
        /// </summary>
        /// <param name="key">key</param>
        public override async Task RemoveAsync(string key)
        {
            try
            {
                if (_redisCacheExtends is not null)
                {
                    await _redisCacheExtends.RemoveAsync(key).ConfigureAwait(false);
                }
                MemoryCache.Remove(key);
            }
            catch (Exception ex)
            {
                _logger?.Information($"尝试移除[{key}]报错.", exception: ex);
            }
        }

        /// <summary>
        /// 刷新一下某个缓存
        /// </summary>
        /// <param name="key">key</param> 
        public override async Task RefreshAsync(string key)
        {

            try
            {
                if (_redisCacheExtends is not null)
                {
                    await _redisCacheExtends.RefreshAsync(key).ConfigureAwait(false);
                }
                _ = MemoryCache.Get(key);
            }
            catch (Exception ex)
            {
                _logger?.Information($"刷新[{key}]报错.", exception: ex);
            }
        }

        /// <summary>
        /// 获取对象的序列化字符串表示。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="data">对象实例</param>
        /// <returns>序列化后的字符串</returns>
        protected override string GetInstanceString<T>(T data)
        {
            return JsonSerializer.Serialize(data, CommonSerializerOptions.CamelCaseChineseNameCaseInsensitive);
        }
    }
}
