using System.Text.RegularExpressions;
using Items.Abstractions.Services;
using Items.Models;

namespace Items.Services
{
    internal sealed class CacheService : ICacheService, IDisposable
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly Dictionary<string, CacheEntry> _cacheContainer;
        private readonly ReaderWriterLock _lock;
        private bool _disposed;

        public CacheService(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            _cacheContainer = new Dictionary<string, CacheEntry>();
            _lock = new ReaderWriterLock();
            _disposed = false;

            Task.Run(async () => {
                while (!_disposed)
                {
                    RemoveExpiredEntries();
                    await Task.Delay(millisecondsDelay: 60_000);
                }
            });
        }

        public void Delete(string regex)
        {
            _lock.AcquireWriterLock(millisecondsTimeout: 100);

            var keysToDelete = _cacheContainer
                .Keys
                .Where(k => new Regex(regex).IsMatch(k))
                .ToArray();

            foreach (var key in keysToDelete)
            {
                _cacheContainer.Remove(key);
            }

            _lock.ReleaseWriterLock();
        }

        public void Set(string key, object value, CacheEntryOptions cacheEntryOptions)
        {
            _lock.AcquireWriterLock(millisecondsTimeout: 100);
            var utcNow = _dateTimeProvider.GetCurrentDateTimeUtc();
            var cacheEntry = new CacheEntry
            {
                CacheEntryOptions = cacheEntryOptions,
                Value = value,
                EnterDateTimeUtc = utcNow,
                ExpirationDateTimeUtc = utcNow.Add(cacheEntryOptions.SlidingExpiration)
            };
            _cacheContainer.Add(key, cacheEntry);
            _lock.ReleaseWriterLock();
        }

        public CacheEntry? TryGetValue<T>(string key, out T? value)
        {
            try
            {
                _lock.AcquireReaderLock(millisecondsTimeout: 100);
                _cacheContainer.TryGetValue(key, out var cacheEntry);

                if (cacheEntry is null)
                {
                    value = default;
                    return null;
                }

                var utcNow = _dateTimeProvider.GetCurrentDateTimeUtc();

                if (cacheEntry.ExpirationDateTimeUtc < utcNow)
                {
                    _lock.UpgradeToWriterLock(millisecondsTimeout: 100);
                    _cacheContainer.Remove(key);
                    _lock.ReleaseWriterLock();
                    value = default;
                    return cacheEntry;
                }

                var newExpirationDateTimeUtc = cacheEntry.ExpirationDateTimeUtc.Add(cacheEntry.CacheEntryOptions.SlidingExpiration);

                cacheEntry.ExpirationDateTimeUtc = newExpirationDateTimeUtc <= cacheEntry.AbsoluteExpirationDateTimeUtc
                    ? newExpirationDateTimeUtc
                    : cacheEntry.AbsoluteExpirationDateTimeUtc;

                value = cacheEntry != null
                    ? (T) cacheEntry.Value
                    : default;

                return cacheEntry;
            }
            catch
            {
                value = default;
                return null;
            }
            finally
            {
                _lock.ReleaseReaderLock();
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }

        private void RemoveExpiredEntries()
        {
            _lock.AcquireWriterLock(millisecondsTimeout: 1000);
            var utcNow = _dateTimeProvider.GetCurrentDateTimeUtc();

            var keysToDelete = _cacheContainer
                .Where(p => utcNow > p.Value.ExpirationDateTimeUtc)
                .Select(p => p.Key)
                .ToArray();

            foreach (var key in keysToDelete)
            {
                _cacheContainer.Remove(key);
            }

            _lock.ReleaseWriterLock();
        }
    }
}
