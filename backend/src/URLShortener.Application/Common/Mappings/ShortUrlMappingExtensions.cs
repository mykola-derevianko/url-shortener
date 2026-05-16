using URLShortener.Application.DTOs.ShortUrl;
using URLShortener.Domain.Entities;

namespace URLShortener.Application.Common.Mappings
{
    public static class ShortUrlMappingExtensions
    {
        public static ShortUrlResponse ToResponse(this ShortUrl item)
        {
            if (item == null) return null!;

            return new ShortUrlResponse
            {
                Id = item.Id,
                OriginalUrl = item.OriginalUrl,
                ShortUrl = $"/s/{item.ShortCode}",
                CreatedByUserId = item.CreatedByUserId,
                CreatedAt = item.CreatedAt
            };
        }
    }
}