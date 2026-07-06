using CSDLNC.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace CSDLNC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "obj", "data-protection-keys")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ThuVienDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CanManageFines", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim("Permission", "Q005") ||
                        context.User.HasClaim("Permission", "Q001")));

                options.AddPolicy("CanViewFineReports", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim("Permission", "Q009") ||
                        context.User.HasClaim("Permission", "Q001")));

                options.AddPolicy("CanManageUsers", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim("Permission", "Q010") ||
                        context.User.HasClaim("Permission", "Q001")));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
