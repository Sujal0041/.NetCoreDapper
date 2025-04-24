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
        private readonly IUserService _userService;
        private readonly JwtSettings _jwtSettings;

        public LoginModel(IUserService userService, IOptions<JwtSettings> jwtSettings)
        {
            _userService = userService;
            _jwtSettings = jwtSettings.Value;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userService.GetUser(Email, Password);
            if (user == null)
            {
                ErrorMessage = "Invalid credentials.";
                return Page();
            }

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim("role", user.Role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                signingCredentials: creds);

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
