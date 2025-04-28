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

        public async Task<IActionResult> OnGetRefreshAsync()
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            string sql = "SELECT * FROM formdata ORDER BY id";
            var entries = await conn.QueryAsync<Entry>(sql);

            var html = new System.Text.StringBuilder();

            foreach (var entry in entries)
            {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{entry.Id}</td>");
                html.AppendLine($"<td>{entry.IsCorporate}</td>");
                html.AppendLine($"<td>{entry.ParentId}</td>");
                html.AppendLine($"<td>{entry.Alias}</td>");
                html.AppendLine($"<td>{entry.FullName}</td>");
                html.AppendLine($"<td>{entry.PuraName}</td>");
                html.AppendLine($"<td>{entry.ShortName}</td>");
                html.AppendLine($"<td>{entry.StreetName}</td>");
                html.AppendLine($"<td>{entry.StreetNameLocale}</td>");
                html.AppendLine($"<td>{entry.WardNumber}</td>");
                html.AppendLine($"<td>{entry.WardNumberLocale}</td>");
                html.AppendLine($"<td>{entry.LocalMNC}</td>");
                html.AppendLine($"<td>{entry.LocalMNC_Locale}</td>");
                html.AppendLine($"<td>{entry.City}</td>");
                html.AppendLine($"<td>{entry.City_Locale}</td>");
                html.AppendLine($"<td>{entry.District}</td>");
                html.AppendLine($"<td>{entry.District_Locale}</td>");
                html.AppendLine("<td>");
                html.AppendLine($"<form method=\"post\" asp-page-handler=\"Delete\">");
                html.AppendLine($"<input type=\"hidden\" name=\"id\" value=\"{entry.Id}\" />");
                html.AppendLine($"<button type=\"submit\" class=\"btn btn-danger btn-sm\" onclick=\"return confirm('Are you sure you want to delete this entry?');\">Delete</button>");
                html.AppendLine($"<a href=\"/upd/{entry.Id}\" class=\"btn btn-warning btn-sm\">Edit</a>");
                html.AppendLine("</form>");
                html.AppendLine("</td>");
                html.AppendLine("</tr>");
            }

            return Content(html.ToString(), "text/html");
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
