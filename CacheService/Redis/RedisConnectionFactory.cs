using System;
using CacheService.Abstract;
using CacheService.AppSettings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace CacheService.Redis
{
    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        private readonly Lazy<ConnectionMultiplexer> _connection;
        public CacheSettings CacheSettings { get; }

        public RedisConnectionFactory(IOptions<CacheSettings> cacheSettings)
        {
            this.CacheSettings = cacheSettings.Value;

            var configurationOptions = new ConfigurationOptions
            {
                EndPoints =
                {
                    CacheSettings.ConnectionString
                },
                ServiceName = CacheSettings.ServiceName,
                AbortOnConnectFail = CacheSettings.AbortOnConnectFail,
                ConnectTimeout = CacheSettings.ConnectTimeout,
                ConnectRetry = CacheSettings.ConnectRetry,
                AsyncTimeout = CacheSettings.AsyncTimeout,
                ClientName = CacheSettings.ClientName
            };

            _connection = new Lazy<ConnectionMultiplexer>(ConnectionMultiplexer.Connect(configurationOptions));
        }

        public ConnectionMultiplexer GetConnection()
        {
            return _connection.Value;
        }
    }
}