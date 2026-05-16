using Microsoft.EntityFrameworkCore;
using URLShortener.Application.Interfaces;
using URLShortener.Domain.Entities;
using URLShortener.Infrastructure.Data;

namespace URLShortener.Infrastructure.Repositories
{
    public class AboutPageRepository : IAboutPageRepository
    {
        private readonly AppDbContext _context;

        public AboutPageRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<AboutPageContent?> GetAsync()
        {
            return _context.AboutPages.AsNoTracking().OrderBy(x => x.Id).FirstOrDefaultAsync();
        }

        public async Task AddAsync(AboutPageContent content)
        {
            await _context.AboutPages.AddAsync(content);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
