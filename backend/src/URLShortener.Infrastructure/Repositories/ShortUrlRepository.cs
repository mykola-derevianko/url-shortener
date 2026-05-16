using Microsoft.EntityFrameworkCore;
using URLShortener.Application.Interfaces;
using URLShortener.Domain.Entities;
using URLShortener.Infrastructure.Data;

namespace URLShortener.Infrastructure.Repositories
{
    public class ShortUrlRepository : IShortUrlRepository
    {
        private readonly AppDbContext _context;

        public ShortUrlRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<ShortUrl?> GetByIdAsync(int id)
        {
            return _context.ShortUrls.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<ShortUrl?> GetByOriginalUrlAsync(string originalUrl)
        {
            return _context.ShortUrls.FirstOrDefaultAsync(x => x.OriginalUrl == originalUrl);
        }

        public Task<ShortUrl?> GetByShortCodeAsync(string shortCode)
        {
            return _context.ShortUrls.FirstOrDefaultAsync(x => x.ShortCode == shortCode);
        }

        public Task<List<ShortUrl>> GetAllAsync()
        {
            return _context.ShortUrls.AsNoTracking().OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task AddAsync(ShortUrl shortUrl)
        {
            await _context.ShortUrls.AddAsync(shortUrl);
        }

        public Task DeleteAsync(ShortUrl shortUrl)
        {
            _context.ShortUrls.Remove(shortUrl);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
