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

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

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
