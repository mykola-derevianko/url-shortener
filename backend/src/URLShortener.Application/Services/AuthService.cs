using URLShortener.Application.DTOs.Auth;
using URLShortener.Application.Interfaces;
using URLShortener.Domain.Entities;
using URLShortener.Domain.Enums;

namespace URLShortener.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<string?> LoginAsync(LoginRequest loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null) return null;
            if (!VerifyPassword(loginDto.Password, user.PasswordHash)) return null;

            return _tokenService.GenerateToken(user);
        }

        public async Task<bool> RegisterAsync(RegisterRequest registerDto, bool isAdmin = false)
        {
            if (await _userRepository.UserExistsByEmailAsync(registerDto.Email))
                return false;

            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                Role = registerDto.RegisterAsAdmin ? Role.Admin : Role.User,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}