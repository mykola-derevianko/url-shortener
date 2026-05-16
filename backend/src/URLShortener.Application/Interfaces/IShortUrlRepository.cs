using URLShortener.Domain.Entities;

namespace URLShortener.Application.Interfaces
{
    public interface IShortUrlRepository
    {
        Task<ShortUrl?> GetByIdAsync(int id);
        Task<ShortUrl?> GetByOriginalUrlAsync(string originalUrl);
        Task<ShortUrl?> GetByShortCodeAsync(string shortCode);
        Task<List<ShortUrl>> GetAllAsync();
        Task AddAsync(ShortUrl shortUrl);
        Task DeleteAsync(ShortUrl shortUrl);
        Task SaveChangesAsync();
    }
}
