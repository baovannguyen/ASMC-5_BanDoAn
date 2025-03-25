using Asm5_BanHoanChinh1.data;
using Asm5_BanHoanChinh1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Asm5_BanHoanChinh1
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Th?i gian h?t h?n c?a session
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/AccountAdmin/AccessDenied"; // Đường dẫn khi bị từ chối truy cập
            });

            // Cấu hình Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();
            // Thêm dịch vụ Authorization và cấu hình Claim Policy
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ViewProductPolicy", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == "AdminViewProduct") ||
                        context.User.HasClaim(c => c.Type == "SalesViewProduct")));
                options.AddPolicy("ManagerPolicy", policy =>
                  policy.RequireAssertion(context =>
                      context.User.HasClaim(c => c.Type == "AdminViewProduct")
                     ));
                options.AddPolicy("UserPolicy", policy =>
              policy.RequireAssertion(context =>
                  context.User.HasClaim(c => c.Type == "UserViewProduct")
                 ));
            });


            var app = builder.Build();
			app.UseSession();
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

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=MonAn}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
