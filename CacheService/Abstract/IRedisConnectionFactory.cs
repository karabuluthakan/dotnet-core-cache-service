using CacheService.AppSettings;
using StackExchange.Redis;

namespace CacheService.Abstract
{
    public interface IRedisConnectionFactory
    {
        ConnectionMultiplexer GetConnection();
        CacheSettings CacheSettings { get; } 
    }
}