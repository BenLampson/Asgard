namespace Asgard.Abstract.Cache
{
    /// <summary>
    /// 下面的三个值,只能三选一! 
    /// </summary>
    public class CacheItemSettings
    {

        /// <summary>
        /// 设置这个值,用来设置这个缓存到某个日期绝对过期
        /// </summary>
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        /// <summary>
        /// 是用这个来设置,距离存储后绝对到期时间
        /// </summary>
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        /// <summary>
        /// 这个时滑动式过期,只要在这个范围内用到了,那就不过期
        /// </summary>
        public TimeSpan? SlidingExpiration { get; set; }

        /// <summary>
        /// 是否存储到所有的缓存中,默认只存储到优先级最高的
        /// </summary>
        public bool SaveToAll { get; set; } = false;
    }
}
