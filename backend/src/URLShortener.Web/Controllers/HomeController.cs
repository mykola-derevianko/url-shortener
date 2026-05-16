using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Application.Interfaces;
using System.Diagnostics;
using URLShortener.Web.Models;

namespace URLShortener.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAboutPageService _aboutPageService;

        public HomeController(IAboutPageService aboutPageService)
        {
            _aboutPageService = aboutPageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> About()
        {
            var result = await _aboutPageService.GetContentAsync();
            var markdown = result.Value ?? string.Empty;
            ViewData["Content"] = Markdown.ToHtml(markdown);
            return View();
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AboutEdit()
        {
            var result = await _aboutPageService.GetContentAsync();
            var model = new AboutEditViewModel
            {
                Content = result.Value ?? string.Empty
            };
            return View(model);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AboutEdit(AboutEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _aboutPageService.UpdateContentAsync(model.Content);
            return RedirectToAction(nameof(About));
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        //TEST AdminOnly policy
        [Authorize(Policy = "AdminOnly")]
        public IActionResult AdminDashboard()
        {
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
