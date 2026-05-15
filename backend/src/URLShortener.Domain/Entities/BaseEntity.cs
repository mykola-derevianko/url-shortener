namespace URLShortener.Domain.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
    }
}