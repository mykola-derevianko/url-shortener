using URLShortener.Application.DTOs;

namespace URLShortener.Application.Interfaces
{
    public interface IAboutPageService
    {
        Task<Result<string>> GetContentAsync();
        Task<Result> UpdateContentAsync(string content);
    }
}
