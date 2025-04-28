using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Dap.Pages
{
    public class HomeModel : PageModel
    {
        // Property to hold user role
        public required string UserRole { get; set; }

        public void OnGet()
        {
            var token = Request.Cookies["jwtToken"];

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);

                    // Extract the user's role from the JWT claims
                    var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role");

                    // Set the value to the property
                    UserRole = roleClaim?.Value ?? "User";
                }
                catch
                {
                    UserRole = "User";
                }
            }
            else
            {
                UserRole = "User";
            }
        }

        public IActionResult OnPostLogout()
        {
            // Clear the JWT token cookie
            Response.Cookies.Delete("jwtToken");

            // Redirect to the home page or login page after logout
            return RedirectToPage("/Index");
        }
    }
}
