namespace URLShortener.Application.DTOs.ShortUrl
{
    public record ShortUrlCreateRequest
    {
        public required string OriginalUrl { get; init; }
    }
}
