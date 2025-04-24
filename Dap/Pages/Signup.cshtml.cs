using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dap.Models;
using Dap.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Dap.Pages
{
    public class SignupModel : PageModel
    {
        private readonly IUserService _userService;

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public SignupModel(IUserService userService)
        {
            _userService = userService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Create the new user
            var newUser = new AppUser
            {
                Username = Username,
                Email = Email,
                PasswordHash = Password, // Store the password directly (not recommended in production)
                Role = "User" // Default role is User
            };

            // Try to register the user
            bool userCreated = await _userService.RegisterUser(newUser);

            if (userCreated)
            {
                return RedirectToPage("/Login");
            }

            ModelState.AddModelError(string.Empty, "Username or Email already exists.");
            return Page();
        }
    }
}
