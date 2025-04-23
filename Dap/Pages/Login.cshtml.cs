using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dap.Services;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Dap.Models;

namespace Dap.Pages // <--- namespace must match the folder structure & project name
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        public LoginModel(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string? Token { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userService.GetUser(Email, Password);

            if (user == null)
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            Token = tokenHandler.WriteToken(token);

            return Content(Token);
        }
    }
}
