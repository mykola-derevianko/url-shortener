using System.ComponentModel.DataAnnotations;

namespace URLShortener.Application.DTOs.Auth
{
    public record LoginRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }

        [Required]
        [MinLength(8)]
        public required string Password { get; init; }
    }
}