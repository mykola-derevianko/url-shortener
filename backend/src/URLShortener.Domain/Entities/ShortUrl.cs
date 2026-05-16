namespace URLShortener.Domain.Entities
{
    public class ShortUrl : BaseEntity
    {
        public required string OriginalUrl { get; set; }
        public required string ShortCode { get; set; }

        public int CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }
    }
}
