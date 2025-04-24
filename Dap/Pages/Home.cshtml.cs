using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dap.Pages
{
    public class HomeModel : PageModel
    {
        public IActionResult OnPostLogout()
        {
            // Delete the correct JWT cookie
            Response.Cookies.Delete("jwtToken");

            // Redirect to login page
            return RedirectToPage("/Login");
        }
    }
}