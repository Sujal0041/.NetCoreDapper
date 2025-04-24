using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace Dap.Pages
{
    public class HomeModel : PageModel
    {
        public IActionResult OnPostLogout()
        {
            // Clear the JWT token cookie
            Response.Cookies.Delete("jwtToken");

            // Redirect to the home page or login page after logout
            return RedirectToPage("/Index");
        }
    }
}
