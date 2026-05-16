using Microsoft.AspNetCore.Mvc;
using URLShortener.Application.Interfaces;

namespace URLShortener.Controllers
{
    [ApiController]
    [Route("s")]
    public class ShortUrlRedirectController : ControllerBase
    {
        private readonly IShortUrlRepository _shortUrlRepository;

        public ShortUrlRedirectController(IShortUrlRepository shortUrlRepository)
        {
            _shortUrlRepository = shortUrlRepository;
        }

        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectToOriginal(string shortCode)
        {
            var item = await _shortUrlRepository.GetByShortCodeAsync(shortCode);
            if (item == null)
            {
                return NotFound();
            }

            return Redirect(item.OriginalUrl);
        }
    }
}
