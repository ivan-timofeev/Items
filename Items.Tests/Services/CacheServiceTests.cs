using Items.Abstractions.Services;
using Items.Models;
using Items.Services;
using Moq;

namespace Items.Tests.Services
{
    [TestClass]
    public class CacheServiceTests
    {
        private readonly CacheService _cacheService;
        private readonly CacheEntryOptions _cacheEntryOptions;
        private readonly DateTime _currentDateTimeUtc;

        public CacheServiceTests()
        {
            _currentDateTimeUtc = new DateTime(2023, 01, 01, 12, 00, 00, DateTimeKind.Utc);

            var dateTimeProviderMock = new Mock<IDateTimeProvider>();

            dateTimeProviderMock
                .Setup(d => d.GetCurrentDateTimeUtc())
                .Returns(_currentDateTimeUtc);

            _cacheService = new CacheService(dateTimeProviderMock.Object);

            _cacheEntryOptions = new CacheEntryOptions
            {
                AbsoluteExpiration = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(1)
            };
        }

        [TestMethod]
        public void Set_HappyPath()
        {
            // Act
            _cacheService.Set("Items:1", "some-cache-value", _cacheEntryOptions);


            // Assert
            _cacheService.TryGetValue("Items:1", out string? value);
            Assert.AreEqual(value, "some-cache-value");
        }

        [TestMethod]
        public void Delete_HappyPath()
        {
            // Arrange
            _cacheService.Set("SomeCacheKeyGroup:1", "some-cache-value-1", _cacheEntryOptions);
            _cacheService.Set("SomeCacheKeyGroup:2", "some-cache-value-2", _cacheEntryOptions);
            _cacheService.Set("SomeCacheKeyGroup:3", "some-cache-value-3", _cacheEntryOptions);
            _cacheService.Set("AnotherSomeCacheKeyGroup", "some-cache-value-4", _cacheEntryOptions);


            // Act
            _cacheService.Delete("SomeCacheKeyGroup:.*");


            // Assert
            _cacheService.TryGetValue("SomeCacheKeyGroup:1", out string? value1);
            Assert.AreEqual(default, value1);
            _cacheService.TryGetValue("SomeCacheKeyGroup:2", out string? value2);
            Assert.AreEqual(default, value2);
            _cacheService.TryGetValue("SomeCacheKeyGroup:3", out string? value3);
            Assert.AreEqual(default, value3);
            _cacheService.TryGetValue("AnotherSomeCacheKeyGroup", out string? categories);
            Assert.AreEqual(categories, "some-cache-value-4");
        }

        [TestMethod]
        public void TryGetValue_HappyPath()
        {
            // Arrange
            _cacheService.Set("some-key", "some-cache-value", _cacheEntryOptions);


            // Act && Assert
            for (int i = 0; i < 11; i++)
            {
                var cacheEntry = _cacheService.TryGetValue("some-key", out string? value);

                Assert.IsNotNull(cacheEntry);

                Assert.AreEqual("some-cache-value", value);

                Assert.AreEqual("some-cache-value", cacheEntry.Value);

                var expirationDateTime = _currentDateTimeUtc.Add((i + 2) * _cacheEntryOptions.SlidingExpiration);

                var expectedExpirationDateTime = expirationDateTime > cacheEntry.AbsoluteExpirationDateTimeUtc
                    ? cacheEntry.AbsoluteExpirationDateTimeUtc
                    : expirationDateTime;

                Assert.AreEqual(
                    expectedExpirationDateTime,
                    cacheEntry.ExpirationDateTimeUtc);
            }
        }
    }
}
