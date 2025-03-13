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
    }
}
