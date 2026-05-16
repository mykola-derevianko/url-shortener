using URLShortener.Application.DTOs;
using URLShortener.Application.Interfaces;
using URLShortener.Domain.Entities;

namespace URLShortener.Application.Services
{
    public class AboutPageService : IAboutPageService
    {
        private readonly IAboutPageRepository _repository;

        public AboutPageService(IAboutPageRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<string>> GetContentAsync()
        {
            var content = await _repository.GetAsync();
            if (content == null)
            {
                return Result<string>.Success(string.Empty);
            }

            return Result<string>.Success(content.Content);
        }

        public async Task<Result> UpdateContentAsync(string content)
        {
            var entity = await _repository.GetAsync();
            if (entity == null)
            {
                entity = new AboutPageContent
                {
                    Content = content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _repository.AddAsync(entity);
            }
            else
            {
                entity.Content = content;
                entity.UpdatedAt = DateTime.UtcNow;
            }

            await _repository.SaveChangesAsync();
            return Result.Success();
        }
    }
}
