using URLShortener.Domain.Entities;

namespace URLShortener.Application.Interfaces
{
    public interface IAboutPageRepository
    {
        Task<AboutPageContent?> GetAsync();
        Task AddAsync(AboutPageContent content);
        Task SaveChangesAsync();
    }
}
