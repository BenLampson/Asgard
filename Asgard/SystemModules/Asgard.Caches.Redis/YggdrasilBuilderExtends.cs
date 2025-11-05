using Asgard.Abstract;

namespace Asgard.Caches.Redis
{
    public static class CacheYggdrasilBuilderExtends
    {
        /// <summary>
        /// 使用Redis+Memory cache作为缓存
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="redisConnString"></param>
        /// <returns></returns>
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
        /// 使用Memory cache作为缓存
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="redisConnString"></param>
        /// <returns></returns>
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
