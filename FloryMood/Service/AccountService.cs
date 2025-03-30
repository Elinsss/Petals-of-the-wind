using Supabase.Gotrue;
using Лепестки_ветра.Models;

namespace Лепестки_ветра.Service
{
    public class AccountService
    {
        private readonly Supabase.Client _client;

        public AccountService(SupabaseClientService clientService)
        {
            _client = clientService.Client;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _client.From<User>()
                    .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, email)
                    .Single();

                if (user == null)
                {
                    return null;                 
                }

                var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.password, password);

                if (result != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
                {
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoginAsync: {ex.Message}");
                return null;
            }
        }


        // Регистрация
        public async Task<User?> RegisterAsync(string email, string password, string your_name, string telephone)
        {
            try
            {
                var existingUser = await _client.From<User>()
                    .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, email)
                    .Single();

                if (existingUser != null)
                {
                    return null; 
                }

                var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
                var hashedPassword = passwordHasher.HashPassword(null, password);

                var newUser = new User
                {
                    email = email,
                    password = hashedPassword,
                    your_name = your_name,
                    telephone = telephone
                };

                var response = await _client.From<User>().Insert(newUser);

                return response.Models.FirstOrDefault(); 
            }
            catch (Exception ex)
            {
                return null; 
            }
        }





        //Корзина
        public async Task MergeCartAsync(int userId, List<cart> sessionCart)
        {
            foreach (var item in sessionCart)
                await AddToCartAsync(userId, item.product_id, item.quantity, item.quantity_flowers);
        }

        public async Task AddToCartAsync(int userId, int productId, int quantity, int quantityFlowers)
        {
            var existingItem = await _client.From<cart>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId)
                .Filter("product_id", Supabase.Postgrest.Constants.Operator.Equals, productId)
                .Single();

            if (existingItem != null)
            {
                existingItem.quantity += quantity; // Увеличиваем количество товара
                existingItem.quantity_flowers = quantityFlowers; // Обновляем количество цветов
                await _client.From<cart>().Update(existingItem); // Обновляем корзину в базе
            }
            else
            {
                await _client.From<cart>().Insert(new cart
                {
                    user_id = userId,
                    product_id = productId,
                    quantity = quantity,
                    quantity_flowers = quantityFlowers // Устанавливаем количество цветов
                });
            }
        }


        public async Task RemoveFromCartAsync(int userId, int productId)
        {
            await _client.From<cart>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId)
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, productId)
                .Delete();
        }



        public async Task<List<cart>> GetCartItemsAsync(int userId)
        {
            // Получаем все элементы корзины для пользователя
            var cartItems = await _client.From<cart>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId)
                .Get();

            // Для каждого элемента корзины загружаем соответствующий продукт
            foreach (var cartItem in cartItems.Models)
            {
                var product = await _client.From<Product>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, cartItem.product_id)
                    .Single();

                // Создаем новый объект, который будет содержать как данные корзины, так и продукт
                cartItem.Product = product;
            }

            return cartItems.Models;
        }





    }
}
