using URLShortener.Domain.Entities;

namespace URLShortener.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
