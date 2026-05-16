using URLShortener.Domain.Entities;

namespace URLShortener.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> UserExistsByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task SaveChangesAsync();
    }
}
