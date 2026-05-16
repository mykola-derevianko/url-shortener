using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using System.Security.Claims;
using URLShortener.Application.DTOs.ShortUrl;
using URLShortener.Application.Interfaces;

namespace URLShortener.Controllers
{
    [ApiController]
    [Route("api/short-urls")]
    public class ShortUrlsController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;

        public ShortUrlsController(IShortUrlService shortUrlService)
        {
            _shortUrlService = shortUrlService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ShortUrlResponse>>> GetAll()
        {
            var result = await _shortUrlService.GetAllAsync();
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = result.ErrorMessage });
            }

            return Ok(result.Value);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ShortUrlResponse>> GetById(int id)
        {
            var result = await _shortUrlService.GetByIdAsync(id);
            if (!result.Succeeded)
            {
                return NotFound(new { message = result.ErrorMessage });
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ShortUrlResponse>> Create([FromBody] ShortUrlCreateRequest request)
        {
            var userId = GetUserId();
            var result = await _shortUrlService.CreateAsync(request, userId);
            if (!result.Succeeded)
            {
                return Conflict(new { message = result.ErrorMessage });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Value?.Id }, result.Value);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _shortUrlService.DeleteAsync(id, userId, isAdmin);
            if (!result.Succeeded)
            {
                return result.ErrorCode switch
                {
                    "NotFound" => NotFound(new { message = result.ErrorMessage }),
                    "Forbidden" => Forbid(),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, new { message = result.ErrorMessage })
                };
            }

            return NoContent();
        }

        private int GetUserId()
        {
            var value = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(value, out var userId))
            {
                throw new SecurityException("Invalid token");
            }

            return userId;
        }
    }
}
