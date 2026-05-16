using URLShortener.Application.DTOs;
using URLShortener.Application.DTOs.Auth;

namespace URLShortener.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginRequest loginDto);
        Task<bool> RegisterAsync(RegisterRequest registerDto, bool isAdmin = false);
    }
}
