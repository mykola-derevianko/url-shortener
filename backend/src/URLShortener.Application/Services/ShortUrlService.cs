using URLShortener.Application.DTOs;
using URLShortener.Application.DTOs.ShortUrl;
using URLShortener.Application.Interfaces;
using URLShortener.Domain.Entities;

namespace test.Application.Services
{
    public class ShortUrlService : IShortUrlService
    {
        private readonly IShortUrlRepository _repository;

        public ShortUrlService(IShortUrlRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<ShortUrlResponse>>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return Result<List<ShortUrlResponse>>.Success(items.Select(MapToResponse).ToList());
        }

        public async Task<Result<ShortUrlResponse>> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
            {
                return Result<ShortUrlResponse>.Failure("NotFound", "Short URL not found.");
            }

            return Result<ShortUrlResponse>.Success(MapToResponse(item));
        }

        public async Task<Result<ShortUrlResponse>> CreateAsync(ShortUrlCreateRequest request, int userId)
        {
            var shortUrl = new ShortUrl
            {
                OriginalUrl = request.OriginalUrl,
                ShortCode = GenerateShortCode(),
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(shortUrl);
            await _repository.SaveChangesAsync();

            return Result<ShortUrlResponse>.Success(MapToResponse(shortUrl));
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

        private static ShortUrlResponse MapToResponse(ShortUrl item)
        {
            return new ShortUrlResponse
            {
                Id = item.Id,
                OriginalUrl = item.OriginalUrl,
                ShortUrl = $"/s/{item.ShortCode}",
                CreatedByUserId = item.CreatedByUserId,
                CreatedAt = item.CreatedAt
            };
        }
    }
}
