using Moq;
using URLShortener.Application.Interfaces;
using test.Application.Services;

namespace URLShortener.Application.UnitTests
{
    [TestClass]
    public class ShortUrlServiceTests
    {

        [TestMethod]
        public async Task GetAllAsync_ItemsExist_MapsToResponseList()
        {
            var repository = new Mock<IShortUrlRepository>(MockBehavior.Strict);
            var createdAt = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var items = new[]
            {
                new Domain.Entities.ShortUrl
                {
                    Id = 1,
                    OriginalUrl = "https://example.com/one",
                    ShortCode = "abc12345",
                    CreatedByUserId = 10,
                    CreatedAt = createdAt,
                    UpdatedAt = createdAt
                },
                new Domain.Entities.ShortUrl
                {
                    Id = 2,
                    OriginalUrl = "https://example.com/two",
                    ShortCode = "def67890",
                    CreatedByUserId = 20,
                    CreatedAt = createdAt.AddMinutes(5),
                    UpdatedAt = createdAt.AddMinutes(5)
                }
            };

            repository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync([.. items]);

            var service = new ShortUrlService(repository.Object);

            var result = await service.GetAllAsync();

            Assert.IsTrue(result.Succeeded);
            Assert.IsNotNull(result.Value);
            Assert.HasCount(2, result.Value);
            Assert.AreEqual("https://example.com/one", result.Value[0].OriginalUrl);
            Assert.AreEqual("/s/abc12345", result.Value[0].ShortUrl);
            Assert.AreEqual(10, result.Value[0].CreatedByUserId);
            Assert.AreEqual(createdAt, result.Value[0].CreatedAt);
            Assert.AreEqual("https://example.com/two", result.Value[1].OriginalUrl);
            Assert.AreEqual("/s/def67890", result.Value[1].ShortUrl);
            Assert.AreEqual(20, result.Value[1].CreatedByUserId);
            Assert.AreEqual(createdAt.AddMinutes(5), result.Value[1].CreatedAt);
            repository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetByIdAsync_ItemMissing_ReturnsNotFoundFailure()
        {
            var repository = new Mock<IShortUrlRepository>(MockBehavior.Strict);

            repository
                .Setup(repo => repo.GetByIdAsync(42))
                .ReturnsAsync((Domain.Entities.ShortUrl?)null);

            var service = new ShortUrlService(repository.Object);

            var result = await service.GetByIdAsync(42);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("NotFound", result.ErrorCode);
            Assert.AreEqual("Short URL not found.", result.ErrorMessage);
            Assert.IsNull(result.Value);
            repository.Verify(repo => repo.GetByIdAsync(42), Times.Once);
        }

        [TestMethod]
        public async Task GetByIdAsync_ItemExists_ReturnsMappedResponse()
        {
            var repository = new Mock<IShortUrlRepository>(MockBehavior.Strict);
            var createdAt = new DateTime(2026, 2, 3, 4, 5, 6, DateTimeKind.Utc);
            var item = new Domain.Entities.ShortUrl
            {
                Id = 7,
                OriginalUrl = "https://example.com/exists",
                ShortCode = "xyz98765",
                CreatedByUserId = 77,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            };

            repository
                .Setup(repo => repo.GetByIdAsync(7))
                .ReturnsAsync(item);

            var service = new ShortUrlService(repository.Object);

            var result = await service.GetByIdAsync(7);

            Assert.IsTrue(result.Succeeded);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(7, result.Value.Id);
            Assert.AreEqual("https://example.com/exists", result.Value.OriginalUrl);
            Assert.AreEqual("/s/xyz98765", result.Value.ShortUrl);
            Assert.AreEqual(77, result.Value.CreatedByUserId);
            Assert.AreEqual(createdAt, result.Value.CreatedAt);
            repository.Verify(repo => repo.GetByIdAsync(7), Times.Once);
        }

        [TestMethod]
        public async Task CreateAsync_ValidRequest_AddsAndSavesShortUrl()
        {
            var repository = new Mock<IShortUrlRepository>(MockBehavior.Strict);
            var request = new DTOs.ShortUrl.ShortUrlCreateRequest
            {
                OriginalUrl = "https://example.com/new"
            };
            Domain.Entities.ShortUrl? addedEntity = null;

            repository
                .Setup(repo => repo.AddAsync(It.IsAny<Domain.Entities.ShortUrl>()))
                .Callback<Domain.Entities.ShortUrl>(entity => addedEntity = entity)
                .Returns(Task.CompletedTask);
            repository
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var service = new ShortUrlService(repository.Object);

            var result = await service.CreateAsync(request, 55);

            Assert.IsTrue(result.Succeeded);
            Assert.IsNotNull(addedEntity);
            Assert.AreEqual(request.OriginalUrl, addedEntity.OriginalUrl);
            Assert.AreEqual(55, addedEntity.CreatedByUserId);
            Assert.IsFalse(string.IsNullOrWhiteSpace(addedEntity.ShortCode));
            Assert.AreEqual(8, addedEntity.ShortCode.Length);
            Assert.AreNotEqual(default, addedEntity.CreatedAt);
            Assert.AreNotEqual(default, addedEntity.UpdatedAt);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(request.OriginalUrl, result.Value.OriginalUrl);
            Assert.AreEqual($"/s/{addedEntity.ShortCode}", result.Value.ShortUrl);
            Assert.AreEqual(55, result.Value.CreatedByUserId);
            Assert.AreEqual(addedEntity.CreatedAt, result.Value.CreatedAt);
            repository.Verify(repo => repo.AddAsync(It.IsAny<Domain.Entities.ShortUrl>()), Times.Once);
            repository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_ItemMissing_ReturnsNotFoundFailure()
        {
            var repository = new Mock<IShortUrlRepository>(MockBehavior.Strict);

            repository
                .Setup(repo => repo.GetByIdAsync(11))
                .ReturnsAsync((Domain.Entities.ShortUrl?)null);

            var service = new ShortUrlService(repository.Object);

            var result = await service.DeleteAsync(11, 1, false);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("NotFound", result.ErrorCode);
            Assert.AreEqual("Short URL not found.", result.ErrorMessage);
            repository.Verify(repo => repo.GetByIdAsync(11), Times.Once);
            repository.Verify(repo => repo.DeleteAsync(It.IsAny<Domain.Entities.ShortUrl>()), Times.Never);
            repository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public async Task DeleteAsync_NotAdminAndNotOwner_ReturnsForbiddenFailure()
        {
            var repository = new Mock<IShortUrlRepository>(MockBehavior.Strict);
            var item = new Domain.Entities.ShortUrl
            {
                Id = 12,
                OriginalUrl = "https://example.com/forbidden",
                ShortCode = "forbid12",
                CreatedByUserId = 99,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            repository
                .Setup(repo => repo.GetByIdAsync(12))
                .ReturnsAsync(item);

            var service = new ShortUrlService(repository.Object);

            var result = await service.DeleteAsync(12, 1, false);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Forbidden", result.ErrorCode);
            Assert.AreEqual("Not allowed to delete this short URL.", result.ErrorMessage);
            repository.Verify(repo => repo.GetByIdAsync(12), Times.Once);
            repository.Verify(repo => repo.DeleteAsync(It.IsAny<Domain.Entities.ShortUrl>()), Times.Never);
            repository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public async Task DeleteAsync_AdminUser_DeletesAndSavesChanges()
        {
            var repository = new Mock<IShortUrlRepository>(MockBehavior.Strict);
            var item = new Domain.Entities.ShortUrl
            {
                Id = 13,
                OriginalUrl = "https://example.com/delete",
                ShortCode = "delete13",
                CreatedByUserId = 42,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            repository
                .Setup(repo => repo.GetByIdAsync(13))
                .ReturnsAsync(item);
            repository
                .Setup(repo => repo.DeleteAsync(item))
                .Returns(Task.CompletedTask);
            repository
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var service = new ShortUrlService(repository.Object);

            var result = await service.DeleteAsync(13, 1, true);

            Assert.IsTrue(result.Succeeded);
            repository.Verify(repo => repo.GetByIdAsync(13), Times.Once);
            repository.Verify(repo => repo.DeleteAsync(item), Times.Once);
            repository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_OwnerUser_DeletesAndSavesChanges()
        {
            var repository = new Mock<IShortUrlRepository>(MockBehavior.Strict);
            var item = new Domain.Entities.ShortUrl
            {
                Id = 14,
                OriginalUrl = "https://example.com/owner",
                ShortCode = "owner014",
                CreatedByUserId = 7,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            repository
                .Setup(repo => repo.GetByIdAsync(14))
                .ReturnsAsync(item);
            repository
                .Setup(repo => repo.DeleteAsync(item))
                .Returns(Task.CompletedTask);
            repository
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var service = new ShortUrlService(repository.Object);

            var result = await service.DeleteAsync(14, 7, false);

            Assert.IsTrue(result.Succeeded);
            repository.Verify(repo => repo.GetByIdAsync(14), Times.Once);
            repository.Verify(repo => repo.DeleteAsync(item), Times.Once);
            repository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

    }
}
