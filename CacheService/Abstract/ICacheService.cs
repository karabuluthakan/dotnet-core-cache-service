using System;
using System.Threading.Tasks;

namespace CacheService.Abstract
{
    public interface ICacheService : IDisposable
    {
        string Get(string key);
        T Get<T>(string key);
        Task<string> GetAsync(string key);
        Task<T> GetAsync<T>(string key);
        bool IsAdd(string key);
        bool IsExpire(string key);
        TimeSpan? GetExpireTime(string key);
        Task<bool> IsExpireAsync(string key);
        bool Add<T>(string key, T value, int? expireTime = null);
        Task<bool> AddAsync<T>(string key, T value, int? expireTime = null);
        bool UpSert<T>(string key, T value);
        Task<bool> UpSertAsync<T>(string key, T value);
        bool Delete(string key);
        Task<bool> DeleteAsync(string key);
        void DeleteByPattern(string pattern);
        Task DeleteByPatternAsync(string pattern);
        bool IsConnect();
    }
}