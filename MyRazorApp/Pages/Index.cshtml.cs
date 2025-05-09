using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using MyRazorApp.Models;
using MyRazorApp.Helpers;
using System.Data;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;

        public IndexModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Filter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public List<string> SelectedColumns { get; set; } = new();

        public List<ClassInformationTable> TableData { get; set; } = new();

        public int PageSize => 10;
        public int TotalPages { get; set; }

        public IActionResult OnGet()
        {
            if (!IsLoggedIn())
                return RedirectToPage("/Login");

            var allData = GetClassListFromDb();

            var filtered = ApplyFiltering(allData.AsQueryable());
            TotalPages = (int)Math.Ceiling(filtered.Count() / (double)PageSize);

            TableData = filtered
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                }).ToList();

            return Page();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
                return Page();

            using var conn = new SqlConnection(_config.GetConnectionString("SchoolDbConnection"));
            conn.Open();

            if (ClassInfo.Id > 0)
            {
                var updateCmd = new SqlCommand("UPDATE Classes SET ClassName = @ClassName, StudentCount = @StudentCount, Description = @Description, IsActive = @IsActive WHERE Id = @Id", conn);
                updateCmd.Parameters.AddWithValue("@Id", ClassInfo.Id);
                updateCmd.Parameters.AddWithValue("@ClassName", ClassInfo.ClassName);
                updateCmd.Parameters.AddWithValue("@StudentCount", ClassInfo.StudentCount);
                updateCmd.Parameters.AddWithValue("@Description", ClassInfo.Description);
                updateCmd.Parameters.AddWithValue("@IsActive", ClassInfo.IsActive);
                updateCmd.ExecuteNonQuery();
            }
            else
            {
                var insertCmd = new SqlCommand("INSERT INTO Classes (ClassName, StudentCount, Description, IsActive) VALUES (@ClassName, @StudentCount, @Description, @IsActive)", conn);
                insertCmd.Parameters.AddWithValue("@ClassName", ClassInfo.ClassName);
                insertCmd.Parameters.AddWithValue("@StudentCount", ClassInfo.StudentCount);
                insertCmd.Parameters.AddWithValue("@Description", ClassInfo.Description);
                insertCmd.Parameters.AddWithValue("@IsActive", ClassInfo.IsActive);
                insertCmd.ExecuteNonQuery();
            }

            return RedirectToPage(new { Filter, PageNumber, SelectedColumns });
        }

        public IActionResult OnPostDelete(int id)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("SchoolDbConnection"));
            conn.Open();

            var updateCmd = new SqlCommand("UPDATE Classes SET IsActive = 0 WHERE Id = @Id", conn);
            updateCmd.Parameters.AddWithValue("@Id", id);
            updateCmd.ExecuteNonQuery();

            return RedirectToPage(new { Filter, PageNumber, SelectedColumns });
        }


        public IActionResult OnPostEdit(int id)
        {
            var item = GetClassListFromDb().FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                ClassInfo = new ClassInformationModel
                {
                    Id = item.Id,
                    ClassName = item.ClassName,
                    StudentCount = item.StudentCount,
                    Description = item.Description
                };
            }

            OnGet();
            return Page();
        }

        public IActionResult OnPostSelectColumn(string column)
        {
            var updated = Request.Form["SelectedColumns"].ToList();

            if (updated.Contains(column))
                updated.Remove(column);
            else
                updated.Add(column);

            return RedirectToPage(new { Filter, PageNumber, SelectedColumns = updated });
        }

        public IActionResult OnPostExportJson()
        {
            var filtered = ApplyFiltering(GetClassListFromDb().AsQueryable());

            var pageData = filtered
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInformationTable
                {
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                }).ToList();

            var columns = SelectedColumns.Any()
                ? SelectedColumns
                : new List<string> { "ClassName", "StudentCount", "Description" };

            var json = Utils.Instance.ExportAsJson(pageData, columns);
            var fileName = $"class_export_{DateTime.Now:yyyyMMddHHmmss}.json";

            return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", fileName);
        }

        private List<ClassInformationModel> GetClassListFromDb()
        {
            var list = new List<ClassInformationModel>();

            using var conn = new SqlConnection(_config.GetConnectionString("SchoolDbConnection"));
            conn.Open();

            var cmd = new SqlCommand("SELECT * FROM Classes WHERE IsActive = 1", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ClassInformationModel
                {
                    Id = (int)reader["Id"],
                    ClassName = reader["ClassName"]?.ToString() ?? string.Empty,
                    StudentCount = (int)reader["StudentCount"],
                    Description = reader["Description"]?.ToString() ?? string.Empty,
                    IsActive = (bool)reader["IsActive"]
                });
            }

            return list;
        }

        private IQueryable<ClassInformationModel> ApplyFiltering(IQueryable<ClassInformationModel> query)
        {
            if (!string.IsNullOrEmpty(Filter))
                query = query.Where(c => c.ClassName.Contains(Filter, StringComparison.OrdinalIgnoreCase));

            return query;
        }

        private bool IsLoggedIn()
        {
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionId = HttpContext.Session.GetString("session_id");

            Request.Cookies.TryGetValue("username", out var cookieUsername);
            Request.Cookies.TryGetValue("token", out var cookieToken);
            Request.Cookies.TryGetValue("session_id", out var cookieSessionId);

            return sessionUsername == cookieUsername && sessionToken == cookieToken && sessionId == cookieSessionId;
        }
    }
}