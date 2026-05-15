using URLShortener.Domain.Enums;

namespace URLShortener.Domain.Entities
{
    public class User : BaseEntity
    {
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required Role Role { get; set; }
    }
}
