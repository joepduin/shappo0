using Microsoft.AspNetCore.Authentication.Cookies;
using shappo0.Discard;
using shappo0.Repositories;
using shappo0.Repositories.Interfaces;
using shappo0.Services;
using shappo0.Services.Interfaces;

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
            builder.Services.AddSingleton<IPaymentRepository, DbPaymentRepository>();
            builder.Services.AddSingleton<IStaffRepository, DbStaffRepository>();
            builder.Services.AddSingleton<ITablesRepository, DbTablesRepository>();
            builder.Services.AddSingleton<IVisitService, VisitService>();
            builder.Services.AddSingleton<IMenuRepository, DbMenuRepository>();
            builder.Services.AddSingleton<IMenuService, MenuService>();
            builder.Services.AddSingleton<IOrderService, OrderService>();
            builder.Services.AddSingleton<IAccountService, AccountService>();
            builder.Services.AddSingleton<ITableService, TableService>();
            builder.Services.AddSingleton<IKitchenRepository, DbKitchenRepository>();
            builder.Services.AddSingleton<IKitchenService, KitchenService>();
            builder.Services.AddSingleton<IBarRepository, DbBarRepository>();
            builder.Services.AddSingleton<IBarService, BarService>();
            builder.Services.AddSingleton<IFinanceRepository, DbFinanceRepository>();
            builder.Services.AddSingleton<IFinanceService, FinanceService>();
            builder.Services.AddControllersWithViews();

            //Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });
            builder.Services.AddAuthorization();

            //Enable Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

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

            app.UseSession();

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
