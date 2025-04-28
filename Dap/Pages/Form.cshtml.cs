using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Dapper;
using Microsoft.AspNetCore.Authorization;

namespace Dap.Pages
{
    [Authorize(Policy = "AdminOnly")]
    public class FormModel : PageModel
    {
        private readonly IConfiguration _config;

        public FormModel(IConfiguration config)
        {
            _config = config;
            Branch = new Branch
            {
                ParentId = string.Empty,
                Alias = string.Empty,
                FullName = string.Empty,
                PuraName = string.Empty,
                ShortName = string.Empty,
                StreetName = string.Empty,
                StreetNameLocale = string.Empty,
                WardNumberLocale = string.Empty,
                LocalMNC = string.Empty,
                LocalMNC_Locale = string.Empty,
                City = string.Empty,
                City_Locale = string.Empty,
                District = string.Empty,
                District_Locale = string.Empty
            };
        }

        [BindProperty]
        public Branch Branch { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                using var conn = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
                string sql = @"
                    INSERT INTO formdata (
                        IsCorporate, ParentId, Alias, FullName, PuraName, ShortName, StreetName, StreetNameLocale,
                        WardNumber, WardNumberLocale, LocalMNC, LocalMNC_Locale, City, City_Locale, District, District_Locale
                    ) VALUES (
                        @IsCorporate, @ParentId, @Alias, @FullName, @PuraName, @ShortName, @StreetName, @StreetNameLocale,
                        @WardNumber, @WardNumberLocale, @LocalMNC, @LocalMNC_Locale, @City, @City_Locale, @District, @District_Locale
                    );";

                conn.Execute(sql, Branch);
                return RedirectToPage("/Success");
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                ModelState.AddModelError(string.Empty, "An error occurred while saving the data. Please try again.");
                return Page();
            }
        }
    }
}
