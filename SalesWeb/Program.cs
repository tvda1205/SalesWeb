using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SalesWeb.Models;
using SalesWeb.Data;
using SalesWeb.Services;
namespace SalesWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<SalesWebContext>(options =>
                options.UseMySql( builder.Configuration.GetConnectionString("SalesWebContext")
                        ?? throw new InvalidOperationException("Connection string 'SalesWebContext' not found."),
                        new MySqlServerVersion(new Version(8, 0, 40)), // substitua pela versão do MySQL que você está usando
                        optionsBuilder => optionsBuilder.MigrationsAssembly("SalesWeb") // Defina o assembly onde as migrações estão
                        ));

            builder.Services.AddScoped<SeedingService>();
            builder.Services.AddScoped<SellerService>();
            builder.Services.AddScoped<DepartmentService>();
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var supportedCultures = new[] { "en-US", "pt-BR" };
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US"),
                SupportedCultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList(),
                SupportedUICultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList()
            };

            var app = builder.Build();
            app.UseRequestLocalization(localizationOptions);


            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var seedingService = services.GetRequiredService<SeedingService>();
                seedingService.Seed();
            }

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");




            app.Run();
        }
    }
}
