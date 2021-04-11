namespace TenderManagement.Infrastructure
{
    public class CacheConfig
    {
        internal const string Key = "Cache";
        public bool UseRedis { get; set; }
        public string RedisConnection { get; set; }
    }
}
