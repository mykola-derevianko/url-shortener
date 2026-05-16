using URLShortener.Application.DTOs;
using URLShortener.Application.DTOs.ShortUrl;

namespace URLShortener.Application.Interfaces
{
    public interface IShortUrlService
    {
        Task<Result<List<ShortUrlResponse>>> GetAllAsync();
        Task<Result<ShortUrlResponse>> GetByIdAsync(int id);
        Task<Result<ShortUrlResponse>> CreateAsync(ShortUrlCreateRequest request, int userId);
        Task<Result> DeleteAsync(int id, int requestUserId, bool isAdmin);
    }
}
