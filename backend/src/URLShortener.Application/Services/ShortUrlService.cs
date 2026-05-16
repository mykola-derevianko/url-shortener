using EntityFramework.Exceptions.Common;
using URLShortener.Application.DTOs;
using URLShortener.Application.DTOs.ShortUrl;
using URLShortener.Application.Interfaces;
using URLShortener.Domain.Entities;
using URLShortener.Application.Common.Mappings;

namespace test.Application.Services
{
    public class ShortUrlService : IShortUrlService
    {
        private const int MaxShortCodeAttempts = 5;
        private readonly IShortUrlRepository _repository;

        public ShortUrlService(IShortUrlRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<ShortUrlResponse>>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();

            var responses = items.Select(item => item.ToResponse()).ToList();

            return Result<List<ShortUrlResponse>>.Success(responses);
        }

        public async Task<Result<ShortUrlResponse>> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
            {
                return Result<ShortUrlResponse>.Failure("NotFound", "Short URL not found.");
            }

            return Result<ShortUrlResponse>.Success(item.ToResponse());
        }

        public async Task<Result<ShortUrlResponse>> CreateAsync(ShortUrlCreateRequest request, int userId)
        {
            var shortUrl = new ShortUrl
            {
                OriginalUrl = request.OriginalUrl,
                ShortCode = string.Empty,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(shortUrl);

            for (var attempt = 1; attempt <= MaxShortCodeAttempts; attempt++)
            {
                shortUrl.ShortCode = GenerateShortCode();
                try
                {
                    await _repository.SaveChangesAsync();
                    return Result<ShortUrlResponse>.Success(shortUrl.ToResponse());
                }
                catch (UniqueConstraintException)
                {
                    if (attempt == MaxShortCodeAttempts) break;
                }

            }
            return Result<ShortUrlResponse>.Failure(
                "Conflict",
                "Failed to generate a unique short code. Please try again.");

        }

        public async Task<Result> DeleteAsync(int id, int requestUserId, bool isAdmin)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
            {
                return Result.Failure("NotFound", "Short URL not found.");
            }

            if (!isAdmin && item.CreatedByUserId != requestUserId)
            {
                return Result.Failure("Forbidden", "Not allowed to delete this short URL.");
            }

            await _repository.DeleteAsync(item);
            await _repository.SaveChangesAsync();

            return Result.Success();
        }

        private static string GenerateShortCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }
    }
}