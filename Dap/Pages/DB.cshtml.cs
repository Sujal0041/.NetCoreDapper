using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Dap.Pages
{
    public class ViewEntriesModel : PageModel
    {
        private readonly IConfiguration _config;

        public List<Entry> Entries { get; set; } = new List<Entry>();

        public ViewEntriesModel(IConfiguration config)
        {
            _config = config;
        }
        public void OnGet()
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            string sql = "SELECT * FROM formdata ORDER BY id";
            Entries = conn.Query<Entry>(sql).ToList();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                using var conn = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
                string sql = "DELETE FROM formdata WHERE id = @Id;";
                await conn.ExecuteAsync(sql, new { Id = id });
                return RedirectToPage(); // Reload this page after delete
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the data. Please try again.");
                return Page();
            }
        }
    }
    public class Entry
    {
        public int Id { get; set; }
        public bool IsCorporate { get; set; }
        public int ParentId { get; set; }
        public required string Alias { get; set; }
        public required string FullName { get; set; }
        public required string PuraName { get; set; }
        public required string ShortName { get; set; }
        public required string StreetName { get; set; }
        public required string StreetNameLocale { get; set; }
        public required string WardNumber { get; set; }
        public required string WardNumberLocale { get; set; }
        public required string LocalMNC { get; set; }
        public required string LocalMNC_Locale { get; set; }
        public required string City { get; set; }
        public required string City_Locale { get; set; }
        public required string District { get; set; }
        public required string District_Locale { get; set; }
    }
}
