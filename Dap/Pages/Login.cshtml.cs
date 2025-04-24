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
            // Validate user credentials using the user service
            var user = await _userService.GetUser(Email, Password);
            if (user == null)
            {
                // Set error message if credentials are invalid
                ErrorMessage = "Invalid credentials.";
                return Page();
            }

            // Create claims for the JWT token
            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email), // Subject claim with user's email
                    new Claim("role", user.Role), // Custom claim for user role
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique identifier for the token
                };

            // Generate a symmetric security key using the secret key from settings
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            // Create signing credentials using the security key and HMAC-SHA256 algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the JWT token with issuer, audience, claims, expiration, and signing credentials
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer, // Token issuer
                audience: _jwtSettings.Audience, // Token audience
                claims: claims, // Claims to include in the token
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes), // Token expiration time
                signingCredentials: creds // Signing credentials
            );

            // Serialize the token to a string
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Append the token to the response cookies with secure options
            Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
            {
                HttpOnly = true, // Prevent client-side scripts from accessing the cookie
                Secure = true, // Ensure the cookie is sent over HTTPS
                SameSite = SameSiteMode.Strict, // Restrict cross-site requests
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes) // Set cookie expiration
            });

            // Redirect the user to the home page after successful login
            return RedirectToPage("/Home");
        }
    }
}
