using Microsoft.AspNetCore.Mvc;
using Лепестки_ветра.Service;

namespace Лепестки_ветра.Controllers
{
    [Route("Account/Wishlist")]
    public class WishlistController : Controller
    {
        private readonly WishlistService _wishlistService;

        public WishlistController(WishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpPost("AddToWishlist")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = HttpContext.Session.GetInt32("ID");
            if (!userId.HasValue)
                return Unauthorized();

            var success = await _wishlistService.AddToWishlistAsync(userId.Value, productId);
            return success ? Ok() : BadRequest();
        }

        [HttpPost("RemoveFromWishlist")] // <-- Добавил уникальный маршрут
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = HttpContext.Session.GetInt32("ID");
            if (!userId.HasValue)
                return Unauthorized();

            var success = await _wishlistService.RemoveFromWishlistAsync(userId.Value, productId);
            return success ? Ok() : BadRequest();
        }

        [HttpGet("GetWishlist")] // <-- Добавил уникальный маршрут
        public async Task<IActionResult> GetWishlist()
        {
            var userId = HttpContext.Session.GetInt32("ID");
            if (!userId.HasValue)
                return Unauthorized();

            var wishlist = await _wishlistService.GetWishlistAsync(userId.Value);
            return View(wishlist);
        }
    }
}