using System.Diagnostics;
using Лепестки_ветра.Models;
using Microsoft.AspNetCore.Mvc;

namespace Лепестки_ветра.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductService _productService;

        public HomeController(ILogger<HomeController> logger, ProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var product = await _productService.GetProductAsync();

            return View(product);
        }

        public async Task<IActionResult> Catalog()
        {
            var product = await _productService.GetProductAsync();

            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
