using Items.Models;

namespace Items.Abstractions.Services
{
    public interface ICacheService
    {
        void Set(string key, object value, CacheEntryOptions cacheEntryOptions);
        CacheEntry? TryGetValue<T>(string key, out T? value);
        void Delete(string regex);
    }
}
