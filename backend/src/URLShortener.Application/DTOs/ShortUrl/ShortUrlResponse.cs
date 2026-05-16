namespace URLShortener.Application.DTOs.ShortUrl
{
    public record ShortUrlResponse
    {
        public required int Id { get; init; }
        public required string OriginalUrl { get; init; }
        public required string ShortUrl { get; init; }
        public required int CreatedByUserId { get; init; }
        public required DateTime CreatedAt { get; init; }
    }
}
