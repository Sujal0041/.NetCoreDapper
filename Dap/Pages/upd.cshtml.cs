using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using Dapper;

namespace Dap.Pages
{
    public class UpdModel : PageModel
    {
        private readonly IConfiguration _config;

        public UpdModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public Entry Entry { get; set; } = new Entry
        {
            Alias = string.Empty,
            FullName = string.Empty,
            PuraName = string.Empty,
            ShortName = string.Empty,
            StreetName = string.Empty,
            StreetNameLocale = string.Empty,
            WardNumber = string.Empty,
            WardNumberLocale = string.Empty,
            LocalMNC = string.Empty,
            LocalMNC_Locale = string.Empty,
            City = string.Empty,
            City_Locale = string.Empty,
            District = string.Empty,
            District_Locale = string.Empty
        };

        public IActionResult OnGet(int id)
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            string sql = "SELECT * FROM formdata WHERE Id = @Id";
            Entry? result = conn.QueryFirstOrDefault<Entry>(sql, new { Id = id });

            if (result == null)
                return RedirectToPage("/ViewEntries"); // Or display an error

            Entry = result;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            using var conn = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            string sql = @"
                    UPDATE formdata SET
                        IsCorporate = @IsCorporate,
                        ParentId = @ParentId,
                        Alias = @Alias,
                        FullName = @FullName,
                        PuraName = @PuraName,
                        ShortName = @ShortName,
                        StreetName = @StreetName,
                        StreetNameLocale = @StreetNameLocale,
                        WardNumber = @WardNumber,
                        WardNumberLocale = @WardNumberLocale,
                        LocalMNC = @LocalMNC,
                        LocalMNC_Locale = @LocalMNC_Locale,
                        City = @City,
                        City_Locale = @City_Locale,
                        District = @District,
                        District_Locale = @District_Locale
                    WHERE Id = @Id";

            conn.Execute(sql, Entry);

            return RedirectToPage("/DB");
        }
    }
}
