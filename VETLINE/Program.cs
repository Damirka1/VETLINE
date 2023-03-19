using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VETLINE.Models;

namespace VETLINE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<DatabaseContext>()
				.AddSignInManager<SignInManager<ApplicationUser>>()
				.AddDefaultTokenProviders();

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = builder.Configuration["Jwt:Issuer"],
						ValidAudience = builder.Configuration["Jwt:Issuer"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
						ClockSkew = TimeSpan.Zero
					};

					options.Events = new JwtBearerEvents
					{
						OnMessageReceived = context =>
						{
							var token = context.Request.Cookies["auth_token"];
							if (!string.IsNullOrEmpty(token))
							{
								context.Token = token;
							}
							return Task.CompletedTask;
						}
					};
				});

			builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("User", policy => policy.RequireRole("User"));
            });

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			builder.Services.AddDbContext<DatabaseContext>(options =>
			{
				options.UseNpgsql(builder.Configuration["DbContextSettings:ConnectionString"]);
			});

			var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UsePathBase("/test");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

			app.UseAuthentication();
            app.UseAuthorization();
            

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}