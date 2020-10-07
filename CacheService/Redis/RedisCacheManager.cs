using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CacheService.Abstract;
using CacheService.AppSettings;
using CacheService.Extensions;
using StackExchange.Redis;

namespace CacheService.Redis
{
    public class RedisCacheManager : ICacheService
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly object _lock;
        private readonly IDatabase _database;
        private readonly CacheSettings _cacheSettings;

        public RedisCacheManager(IRedisConnectionFactory connectionFactory)
        {
            _connection = connectionFactory.GetConnection();
            _cacheSettings = connectionFactory.CacheSettings;
            _database = _connection.GetDatabase();
            this._lock = new object();
        }

        public bool IsExpire(string key)
        {
            return _database.KeyExpire(key, DateTime.UtcNow);
        }

        public TimeSpan? GetExpireTime(string key)
        {
            return _database.KeyTimeToLive(key);
        }

        public async Task<bool> IsExpireAsync(string key)
        {
            return await _database.KeyExpireAsync(key, DateTime.UtcNow);
        }

        public bool Add<T>(string key, T value, int? expireTime = null)
        {
            var (data, timeSpan) = SetCacheData(value, expireTime);
            lock (_lock)
            {
                return _database.StringSet(key, data, timeSpan, When.NotExists);
            }
        }

        public bool UpSert<T>(string key, T value)
        {
            lock (_lock)
            {
                return _database.StringGetSet(key, value.Serialize()).HasValue;
            }
        }

        public async Task<bool> AddAsync<T>(string key, T value, int? expireTime = null)
        {
            var (data, timeSpan) = SetCacheData(value, expireTime);
            return await _database.StringSetAsync(key, data, timeSpan, When.NotExists);
        }

        public async Task<bool> UpSertAsync<T>(string key, T value)
        {
            return (await _database.StringGetSetAsync(key, value.Serialize())).HasValue;
        }

        public T Get<T>(string key)
        {
            return _database.StringGet(key).Deserialize<T>();
        }

        public string Get(string key)
        {
            return _database.StringGet(key);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return (await _database.StringGetAsync(key)).Deserialize<T>();
        }

        public async Task<string> GetAsync(string key)
        { 
            return await _database.StringGetAsync(key);
        }

        public bool IsAdd(string key)
        {
            return _database.KeyExists(key);
        }

        public bool Delete(string key)
        {
            lock (_lock)
            {
                return this.IsAdd(key) && _database.KeyDelete(key);
            }
        }

        public async Task<bool> DeleteAsync(string key)
        {
            if (this.IsAdd(key))
            {
                return await _database.KeyDeleteAsync(key);
            }

            return false;
        }

        public void DeleteByPattern(string pattern)
        {
            var keysToRemove = this.GetRemoveKeys(pattern);

            if (keysToRemove.Any())
            {
                foreach (var key in keysToRemove)
                {
                    lock (_lock)
                    {
                        _database.KeyDelete(key);
                    }
                }
            }
        }

        public async Task DeleteByPatternAsync(string pattern)
        {
            var keysToRemove = this.GetRemoveKeys(pattern);
            if (keysToRemove.Any())
            {
                foreach (var key in keysToRemove)
                {
                    await _database.KeyDeleteAsync(key);
                }
            }
        }

        public bool IsConnect()
        {
            return _connection.IsConnecting;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }

        private List<RedisKey> GetRemoveKeys(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var endPoint = _connection.GetEndPoints().First();
            var keys = _connection.GetServer(endPoint).Keys();
            return keys.Where(x => regex.IsMatch(x.ToString())).ToList();
        }

        private Tuple<string, TimeSpan> SetCacheData(object value, int? expireTime = null)
        {
            TimeSpan timeSpan;
            if (expireTime.HasValue)
            {
                timeSpan = expireTime < 0
                    ? TimeSpan.FromMinutes(_cacheSettings.DefaultExpireTime)
                    : TimeSpan.FromMinutes((double) expireTime);
            }
            else
            {
                timeSpan = TimeSpan.FromMinutes(_cacheSettings.DefaultExpireTime);
            }

            var data = value.Serialize();

            return Tuple.Create(data, timeSpan);
        }
    }
}