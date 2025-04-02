using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Лепестки_ветра.Service;
using Лепестки_ветра.Models;
using Newtonsoft.Json;

namespace Лепестки_ветра.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _authService;
        private readonly WishlistService _wishlistService;


        // Единый конструктор
        public AccountController(AccountService authService, WishlistService wishlistService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _wishlistService = wishlistService ?? throw new ArgumentNullException(nameof(wishlistService));
        }

        public IActionResult login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email и пароль обязательны.");
                return View();
            }

            try
            {
                var user = await _authService.LoginAsync(email, password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Неверный email или пароль.");
                    return View();
                }

                // Сохранение данных пользователя в сессию
                HttpContext.Session.SetInt32("ID", user.id);
                HttpContext.Session.SetString("Email", user.email);
                HttpContext.Session.SetString("YourName", user.your_name);
                HttpContext.Session.SetString("Telephone", user.telephone);
                HttpContext.Session.SetString("Adress", user.delivery_address);

                // Получаем корзину из сессии, если она есть
                var cart = HttpContext.Session.GetObjectFromJson<List<cart>>("Cart");

                if (cart != null && cart.Any())
                {
                    // Переносим корзину гостя в БД для зарегистрированного пользователя
                    foreach (var cartItem in cart)
                    {
                        await _authService.AddToCartAsync(user.id, cartItem.product_id, cartItem.quantity, cartItem.quantity_flowers);
                    }
                }

                // Редирект на главную страницу
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                ModelState.AddModelError("", "Произошла ошибка. Попробуйте позже.");
                return View();
            }
        }

        public IActionResult register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string your_name, string telephone)
        {
            var user = await _authService.RegisterAsync(email, password, your_name, telephone);

            if (user != null)
            {
                // Получаем корзину из сессии, если она есть
                var cart = HttpContext.Session.GetObjectFromJson<List<cart>>("Cart");

                if (cart != null && cart.Any())
                {
                    // Переносим корзину гостя в БД для зарегистрированного пользователя
                    foreach (var cartItem in cart)
                    {
                        await _authService.AddToCartAsync(user.id, cartItem.product_id, cartItem.quantity, cartItem.quantity_flowers);
                    }

                    // Очищаем корзину из сессии после того, как она перенесена в базу данных
                    HttpContext.Session.Remove("Cart");
                }

                return RedirectToAction("Login", "Account");
            }

            // Ошибка при регистрации
            ModelState.AddModelError("", "Ошибка при регистрации.");
            return View();
        }

        // Метод для выхода из аккаунта
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var userId = HttpContext.Session.GetInt32("ID");

            if (userId.HasValue)
            {
                // Выход пользователя из системы
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
            }
            else
            {
                HttpContext.Session.Remove("ID");
                HttpContext.Session.Remove("Email");
                HttpContext.Session.Remove("YourName");
                HttpContext.Session.Remove("Telephone");
                HttpContext.Session.Remove("Adress");
            }

            return RedirectToAction("Index", "Home");
        }


        public IActionResult profile()
        {
            return View();
        }

        //Корзина
        public async Task<IActionResult> cart()
        {
            var userId = HttpContext.Session.GetInt32("ID");
            List<cart> cartItems;

            if (userId.HasValue)
            {
                cartItems = await _authService.GetCartItemsAsync(userId.Value);
            }
            else
            {
                cartItems = HttpContext.Session.GetObjectFromJson<List<cart>>("Cart") ?? new List<cart>();

                foreach (var item in cartItems)
                {
                    item.Product = await _authService.GetProductByIdAsync(item.product_id); 
                }
            }

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1, int quantityFlowers = 5)
        {
            var userId = HttpContext.Session.GetInt32("ID");

            if (userId.HasValue) 
            {
                await _authService.AddToCartAsync(userId.Value, productId, quantity, quantityFlowers);
            }
            else 
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<cart>>("Cart") ?? new List<cart>();

                var item = cart.FirstOrDefault(c => c.product_id == productId);
                if (item != null)
                {
                    item.quantity += quantity; 
                    item.quantity_flowers = quantityFlowers; 
                }
                else
                {
                    cart.Add(new cart { product_id = productId, quantity = quantity, quantity_flowers = quantityFlowers });
                }

                // Сохраняем корзину обратно в сессию
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToAction("Cart");  
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = HttpContext.Session.GetInt32("ID");

            if (userId.HasValue) // Авторизованный пользователь
            {
                await _authService.RemoveFromCartAsync(userId.Value, id);
            }
            else // Гость
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<cart>>("Cart") ?? new List<cart>();
                cart.RemoveAll(item => item.id == id);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveOneFromCart(int id)
        {
            var userId = HttpContext.Session.GetInt32("ID");

            if (userId.HasValue) // Авторизованный пользователь
            {
                await _authService.DecreaseCartItemQuantityAsync(userId.Value, id);
            }
            else // Гость
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<cart>>("Cart") ?? new List<cart>();
                var item = cart.FirstOrDefault(c => c.id == id);

                if (item != null)
                {
                    if (item.quantity > 1)
                    {
                        item.quantity--; // Уменьшаем количество
                    }
                    else
                    {
                        cart.Remove(item); // Если 1 товар, удаляем его
                    }
                }

                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = HttpContext.Session.GetInt32("ID");
            if (!userId.HasValue)
                return RedirectToAction("login","account");

            var success = await _wishlistService.AddToWishlistAsync(userId.Value, productId);

            if (success)
            {
                // Успешно добавили в избранное
                TempData["Message"] = "Товар добавлен в избранное!";

            }
            else
            {
                // Товар уже в избранном
                TempData["ErrorMessage"] = "Товар уже в избранном!";
            }

            return RedirectToAction("wishlist", "account");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = HttpContext.Session.GetInt32("ID");
            if (!userId.HasValue)
                return RedirectToAction("login", "account");

            var success = await _wishlistService.RemoveFromWishlistAsync(userId.Value, productId);

            if (success)
            {
                return RedirectToAction("wishlist", "account");
            }
            else
            {
                return BadRequest("Не удалось удалить из избранного.");
            }
        }



        public async Task<IActionResult> wishlist()
        {
            var userId = HttpContext.Session.GetInt32("ID");
            if (!userId.HasValue)
                return Unauthorized();

            var wishlist = await _wishlistService.GetWishlistAsync(userId.Value) ?? new List<Wishlist>();

            return View(wishlist);
        }
    }
}
