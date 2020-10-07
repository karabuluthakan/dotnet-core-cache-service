using CacheService.Abstract;
using CacheService.AppSettings;
using CacheService.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CacheService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisConfiguration(this IServiceCollection services,
            IConfiguration configuration, string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = nameof(CacheSettings);
            }

            return services
                .Configure<CacheSettings>(options => configuration.GetSection(name).Bind(options))
                .AddScoped<IRedisConnectionFactory, RedisConnectionFactory>()
                .AddScoped<ICacheService, RedisCacheManager>();
        }
    }
}