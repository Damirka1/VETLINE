using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VETLINE.Models;

namespace VETLINE.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        private static readonly string[] Roles = new string[] { "Admin", "User" };

		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            ILogger<HomeController> logger, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _configuration = configuration;

            CreateRoles().Wait();
		}

        private async Task CreateRoles()
        {
			// Create the default roles if they don't already exist
			foreach (string roleName in Roles)
			{
				if (!await _roleManager.RoleExistsAsync(roleName))
				{
					var role = new IdentityRole { Name = roleName };
					await _roleManager.CreateAsync(role);
				}
			}
		}

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
			if (!ModelState.IsValid)
            {
                return View(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = model.FullName,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(ModelState);
            }

			// Assign default user role to the new user
			await _userManager.AddToRoleAsync(user, "User");

            string token = GenerateJwtToken(user).Result;

            Response.Cookies.Append("auth_token", token, new CookieOptions { HttpOnly = true });

            return RedirectToLocal(returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(ModelState);
            }

            string token = GenerateJwtToken(user).Result;

			Response.Cookies.Append("auth_token", token, new CookieOptions { HttpOnly = true });
			return RedirectToLocal(returnUrl);
		}

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			_logger.LogInformation("User logged out.");
            Response.Cookies.Delete("auth_token");
            return RedirectToLocal(null);
		}
	}
}
