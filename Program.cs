using Microsoft.AspNetCore.Authentication.Cookies;
using shappo0.Repositories;
using shappo0.Services;

namespace shappo0
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services and repositories to the container.
            builder.Services.AddSingleton<IOrderRepository, DbOrderRepository>();
            builder.Services.AddSingleton<IOrderLineRepository, DbOrderLineRepository>();
            builder.Services.AddSingleton<IVisitRepository, DbVisitRepository>();
            builder.Services.AddSingleton<IPaymentService, PaymentService>();
            // Registratie van de payment repository zodat de PaymentService deze via DI kan krijgen
            builder.Services.AddSingleton<IPaymentRepository, DbPaymentRepository>();
            builder.Services.AddSingleton<IUsersRepository, DbUsersRepository>();
            builder.Services.AddSingleton<ITablesRepository, DbTablesRepository>();
            builder.Services.AddSingleton<IMenuRepository, DbMenuRepository>();
            builder.Services.AddSingleton<IMenuService, MenuService>();
            builder.Services.AddSingleton<IAccountService, AccountService>();
            builder.Services.AddSingleton<ITableService, TableService>();
            builder.Services.AddSingleton<IKitchenRepository, DbKitchenRepository>();
            builder.Services.AddSingleton<IKitchenService, KitchenService>();
            builder.Services.AddSingleton<IBarRepository, DbBarRepository>();
            builder.Services.AddSingleton<IBarService, BarService>();
            builder.Services.AddControllersWithViews();

            //Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            var supportedCultures = new[] { "en-US" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            app.Run();
        }
    }
}
