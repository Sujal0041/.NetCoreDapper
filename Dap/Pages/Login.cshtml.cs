using Dap.Models;
using Dap.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Dap.Pages
{
    public class LoginModel : PageModel
    {
        // Dependency injection for user service to handle user-related operations
        private readonly IUserService _userService;

        // Dependency injection for JWT settings to configure token generation
        private readonly JwtSettings _jwtSettings;

        // Constructor to initialize dependencies
        public LoginModel(IUserService userService, IOptions<JwtSettings> jwtSettings)
        {
            _userService = userService;
            _jwtSettings = jwtSettings.Value;
        }

        // Property to bind the email input from the login form
        [BindProperty]
        public string Email { get; set; }

        // Property to bind the password input from the login form
        [BindProperty]
        public string Password { get; set; }

        // Property to store error messages for invalid login attempts
        public string ErrorMessage { get; set; }

        // Handles GET requests to render the login page
        public IActionResult OnGet()
        {
            // Prevent browser from caching this page
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            var token = Request.Cookies["jwtToken"];
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Home");
            }

            return Page();
        }

        // Handles POST requests for login form submission
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userService.GetUser(Email, Password);
            if (user == null)
            {
                ErrorMessage = "Invalid credentials.";
                return Page();
            }

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes)
            });

            return RedirectToPage("/Home");
        }

    }
}
