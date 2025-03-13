using Лепестки_ветра;
using Microsoft.AspNetCore.Authentication.Cookies;
using Лепестки_ветра.Service;

internal class Program
{


    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddSingleton(new SupabaseClientService(
            builder.Configuration["Supabase:Url"],
            builder.Configuration["Supabase:Key"]
        ));

        // Регистрация пользовательских сервисов
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<AccountService>();




        // Подключение сессий
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true; // Доступ только через HTTP
            options.Cookie.IsEssential = true; // Куки обязательны для работы приложения
        });


        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/account/login"; // Путь к странице входа
                options.LogoutPath = "/account/logout"; // Путь к действию выхода
            });

        // Регистрация HttpContextAccessor для доступа к текущему контексту
        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts(); 
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseCors("AllowAll"); // CORS до аутентификации
        app.UseSession(); // Сначала сессии
        app.UseAuthentication(); // Затем аутентификация
        app.UseAuthorization(); // И потом авторизация


        app.MapDefaultControllerRoute();


        app.UseDeveloperExceptionPage();

        app.Run();
    }
}