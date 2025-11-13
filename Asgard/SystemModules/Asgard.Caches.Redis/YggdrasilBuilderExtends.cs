using Asgard.Abstract;

namespace Asgard.Caches.Redis
{
    /// <summary>
    /// 世界之树建造者的 Redis 缓存扩展方法。
    /// </summary>
    public static class CacheYggdrasilBuilderExtends
    {
        /// <summary>
        /// 使用 Redis+Memory cache 作为缓存。
        /// </summary>
        /// <param name="builder">世界之树建造者</param>
        /// <param name="redisConnString">Redis 连接字符串</param>
        /// <returns>世界之树建造者</returns>
        public static YggdrasilBuilder UseRedisCache(this YggdrasilBuilder builder, string redisConnString)
        {
            return builder.SetCacheManager((loggerProvider, nodeConfig) =>
            {
                var cacheManager = new CacheManager(loggerProvider);
                cacheManager.PushRedis(redisConnString, loggerProvider?.CreateLogger("CacheManager"));
                return cacheManager;
            });
        }

        /// <summary>
        /// 使用 Memory cache 作为缓存。
        /// </summary>
        /// <param name="builder">世界之树建造者</param>
        /// <returns>世界之树建造者</returns>
        public static YggdrasilBuilder UseMemCache(this YggdrasilBuilder builder)
        {
            return builder.SetCacheManager((loggerProvider, _) =>
            {
                var cacheManager = new CacheManager(loggerProvider);
                return cacheManager;
            });
        }
    }
}
