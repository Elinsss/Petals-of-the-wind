using ��������_�����;
using Microsoft.AspNetCore.Authentication.Cookies;
using ��������_�����.Service;

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

        // ����������� ���������������� ��������
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<AccountService>();




        // ����������� ������
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true; // ������ ������ ����� HTTP
            options.Cookie.IsEssential = true; // ���� ����������� ��� ������ ����������
        });


        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/account/login"; // ���� � �������� �����
                options.LogoutPath = "/account/logout"; // ���� � �������� ������
            });

        // ����������� HttpContextAccessor ��� ������� � �������� ���������
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
        app.UseCors("AllowAll"); // CORS �� ��������������
        app.UseSession(); // ������� ������
        app.UseAuthentication(); // ����� ��������������
        app.UseAuthorization(); // � ����� �����������


        app.MapDefaultControllerRoute();


        app.UseDeveloperExceptionPage();

        app.Run();
    }
}