using Microsoft.EntityFrameworkCore;
using URLShortener.Domain.Entities;
using URLShortener.Infrastructure.Data;
using URLShortener.Infrastructure.Repositories;

namespace URLShortener.Application.UnitTests.Repositories
{
    [TestClass]
    public class ShortUrlRepositoryTests
    {
        [TestMethod]
        public void Constructor_ValidContext_CreatesInstance()
        {
            var options = CreateOptions();
            using var context = new AppDbContext(options);

            var repository = new ShortUrlRepository(context);

            Assert.IsNotNull(repository);
        }

        [TestMethod]
        public async Task GetByIdAsync_EntityMissing_ReturnsNull()
        {
            var options = CreateOptions();
            await using var context = new AppDbContext(options);
            var repository = new ShortUrlRepository(context);

            var result = await repository.GetByIdAsync(999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByOriginalUrlAsync_EntityExists_ReturnsEntity()
        {
            var options = CreateOptions();
            var shortUrl = new ShortUrl
            {
                OriginalUrl = "https://example.com/original",
                ShortCode = "orig123",
                CreatedAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 2, 2, 0, 0, 0, DateTimeKind.Utc)
            };

            await using (var context = new AppDbContext(options))
            {
                context.ShortUrls.Add(shortUrl);
                await context.SaveChangesAsync();
            }

            await using var queryContext = new AppDbContext(options);
            var repository = new ShortUrlRepository(queryContext);

            var result = await repository.GetByOriginalUrlAsync(shortUrl.OriginalUrl);

            Assert.IsNotNull(result);
            Assert.AreEqual(shortUrl.OriginalUrl, result.OriginalUrl);
            Assert.AreEqual(shortUrl.ShortCode, result.ShortCode);
        }

        [TestMethod]
        public async Task GetByOriginalUrlAsync_EntityMissing_ReturnsNull()
        {
            var options = CreateOptions();
            await using var context = new AppDbContext(options);
            var repository = new ShortUrlRepository(context);

            var result = await repository.GetByOriginalUrlAsync("https://example.com/missing");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByShortCodeAsync_EntityExists_ReturnsEntity()
        {
            var options = CreateOptions();
            var shortUrl = new ShortUrl
            {
                OriginalUrl = "https://example.com/short",
                ShortCode = "short42",
                CreatedAt = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 3, 2, 0, 0, 0, DateTimeKind.Utc)
            };

            await using (var context = new AppDbContext(options))
            {
                context.ShortUrls.Add(shortUrl);
                await context.SaveChangesAsync();
            }

            await using var queryContext = new AppDbContext(options);
            var repository = new ShortUrlRepository(queryContext);

            var result = await repository.GetByShortCodeAsync(shortUrl.ShortCode);

            Assert.IsNotNull(result);
            Assert.AreEqual(shortUrl.ShortCode, result.ShortCode);
            Assert.AreEqual(shortUrl.OriginalUrl, result.OriginalUrl);
        }

        [TestMethod]
        public async Task GetByShortCodeAsync_EntityMissing_ReturnsNull()
        {
            var options = CreateOptions();
            await using var context = new AppDbContext(options);
            var repository = new ShortUrlRepository(context);

            var result = await repository.GetByShortCodeAsync("missing-code");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetAllAsync_MultipleEntities_ReturnsOrderedList()
        {
            var options = CreateOptions();
            var older = new ShortUrl
            {
                OriginalUrl = "https://example.com/older",
                ShortCode = "old1",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            var newer = new ShortUrl
            {
                OriginalUrl = "https://example.com/newer",
                ShortCode = "new1",
                CreatedAt = new DateTime(2024, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 4, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            await using (var context = new AppDbContext(options))
            {
                context.ShortUrls.AddRange(older, newer);
                await context.SaveChangesAsync();
            }

            await using var queryContext = new AppDbContext(options);
            var repository = new ShortUrlRepository(queryContext);

            var result = await repository.GetAllAsync();

            Assert.HasCount(2, result);
            Assert.AreEqual(newer.OriginalUrl, result[0].OriginalUrl);
            Assert.AreEqual(older.OriginalUrl, result[1].OriginalUrl);
        }

        [TestMethod]
        public async Task GetAllAsync_NoEntities_ReturnsEmptyList()
        {
            var options = CreateOptions();
            await using var context = new AppDbContext(options);
            var repository = new ShortUrlRepository(context);

            var result = await repository.GetAllAsync();

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        public async Task GetByIdAsync_EntityExists_ReturnsEntity()
        {
            var options = CreateOptions();
            var shortUrl = new ShortUrl
            {
                Id = 42,
                OriginalUrl = "https://example.com/original",
                ShortCode = "abc123",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            };

            await using (var context = new AppDbContext(options))
            {
                context.ShortUrls.Add(shortUrl);
                await context.SaveChangesAsync();
            }

            await using var queryContext = new AppDbContext(options);
            var repository = new ShortUrlRepository(queryContext);

            var result = await repository.GetByIdAsync(42);

            Assert.IsNotNull(result);
            Assert.AreEqual(shortUrl.Id, result.Id);
            Assert.AreEqual(shortUrl.OriginalUrl, result.OriginalUrl);
            Assert.AreEqual(shortUrl.ShortCode, result.ShortCode);
        }

        [TestMethod]
        public async Task AddAsync_ValidEntity_AddsToContext()
        {
            var options = CreateOptions();
            var shortUrl = new ShortUrl
            {
                OriginalUrl = "https://example.com/add",
                ShortCode = "add123",
                CreatedAt = new DateTime(2024, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 5, 2, 0, 0, 0, DateTimeKind.Utc)
            };

            await using var context = new AppDbContext(options);
            var repository = new ShortUrlRepository(context);

            await repository.AddAsync(shortUrl);

            Assert.IsTrue(context.ShortUrls.Local.Contains(shortUrl));
        }

        [TestMethod]
        public async Task DeleteAsync_ExistingEntity_RemovesEntity()
        {
            var options = CreateOptions();
            var shortUrl = new ShortUrl
            {
                OriginalUrl = "https://example.com/delete",
                ShortCode = "del123",
                CreatedAt = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 6, 2, 0, 0, 0, DateTimeKind.Utc)
            };

            await using var context = new AppDbContext(options);
            context.ShortUrls.Add(shortUrl);
            await context.SaveChangesAsync();
            var repository = new ShortUrlRepository(context);

            await repository.DeleteAsync(shortUrl);
            await repository.SaveChangesAsync();

            Assert.AreEqual(0, await context.ShortUrls.CountAsync());
        }

        private static DbContextOptions<AppDbContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase($"ShortUrlRepositoryTests-{Guid.NewGuid()}")
                .Options;
        }
    }
}
