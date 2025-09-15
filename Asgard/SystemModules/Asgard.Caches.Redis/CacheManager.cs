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
    /// 缓存管理对象
    /// </summary>
    public class CacheManager : AbsCache
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly AbsLogger _logger;


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
        public CacheManager(AbsLoggerProvider loggerProvider) : base(loggerProvider)
        {
            _logger = loggerProvider.CreateLogger<CacheManager>();
        }

        /// <summary>
        /// 推送一个新的缓存实例
        /// </summary> 
        public void PushRedis(string redisConnString, AbsLogger logger)
        {
            if (string.IsNullOrEmpty(redisConnString))
            {
                logger.Warning($"Redis模块关闭,因为没有配置连接字符串.");
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
                logger.Critical($"Redis服务关创建失败,模块关闭", exception: ex);
            }
        }


        /// <summary>
        /// 根据key获取某一个缓存,自己用优先级判定
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="key">key</param> 
        /// <returns>从缓存获取的值,可能为空</returns>
        public override T? TryGet<T>(string key) where T : class
        {
            try
            {
                var res = _redisCacheExtends?.GetString(key) ?? Encoding.UTF8.GetString(MemoryCache.Get<byte[]>(key) ?? []);
                if (!string.IsNullOrWhiteSpace(res))
                {
                    return JsonSerializer.Deserialize<T>(res, CommonSerializerOptions.CamelCaseChineseNameCaseInsensitive)!;
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
        public override string? TryGetString(string key)
        {
            try
            {
                return _redisCacheExtends?.GetString(key) ?? Encoding.UTF8.GetString(MemoryCache.Get<byte[]>(key) ?? []); ;
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
        public override bool Exists(string key)
        {
            try
            {
                var rawBytes = _redisCacheExtends?.Get(key) ?? MemoryCache.Get<byte[]>(key);
                return rawBytes is null;
            }
            catch (Exception ex)
            {
                _logger?.Information($"尝试获取缓存 {key} 报错.", exception: ex);
            }
            return default;
        }






        /// <summary>
        /// 保存一个字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="settings"></param>
        public override void SetString(string key, string value, CacheItemSettings settings)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            if (_redisCacheExtends is null)
            {
                var opt = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = settings.AbsoluteExpiration,
                    AbsoluteExpirationRelativeToNow = settings.AbsoluteExpirationRelativeToNow,
                    SlidingExpiration = settings.SlidingExpiration,
                };
                _ = MemoryCache.Set(key, bytes, opt);
            }
            else
            {
                var cacheSettings = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = settings.AbsoluteExpiration,
                    AbsoluteExpirationRelativeToNow = settings.AbsoluteExpirationRelativeToNow,
                    SlidingExpiration = settings.SlidingExpiration,
                };
                _redisCacheExtends?.Set(key, bytes, cacheSettings);
            }
        }

        /// <summary>
        /// 根据某一个匹配模型获取所有key
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public override string[] GetKeys(string pattern)
        {
            try
            {
                if (_redis is not null)
                {
                    return _redis.Keys(pattern);
                }
                else
                {
                    var realPattern = pattern.Replace("*", "");
                    return MemoryCache.Keys.Select(item => item.ToString()).ToList().Where(item => item?.StartsWith(realPattern) ?? false)
                        .Select(item => item ?? "BadKeyData")
                        .ToArray() ?? [];
                }
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
        public override void Remove(string key)
        {
            try
            {
                if (_redisCacheExtends is null)
                {
                    MemoryCache.Remove(key);
                }
                else
                {
                    _redisCacheExtends?.Remove(key);
                }
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
        public override void Refresh(string key)
        {

            try
            {
                if (_redisCacheExtends is null)
                {
                    _ = MemoryCache.Get(key);
                }
                else
                {
                    _redisCacheExtends.Refresh(key);
                }
            }
            catch (Exception ex)
            {
                _logger?.Information($"刷新[{key}]报错.", exception: ex);
            }
        }

        protected override string GetInstanceString<T>(T data)
        {
            return JsonSerializer.Serialize(data, CommonSerializerOptions.CamelCaseChineseNameCaseInsensitive);
        }
    }
}
