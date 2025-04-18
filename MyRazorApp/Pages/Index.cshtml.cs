using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using MyRazorApp.Helpers;
using System.Text.Json;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> ClassList { get; set; } = new();

        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Filter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty]
        public List<string> SelectedColumns { get; set; } = new();

        [BindProperty]
        public string ExportType { get; set; } = "all";

        public List<ClassInformationTable> TableData { get; set; } = new();

        public int PageSize => 10;
        public int TotalPages { get; set; }

        public void OnGet()
        {
            if (!ClassList.Any())
            {
                for (int i = 1; i <= 120; i++)
                {
                    ClassList.Add(new ClassInformationModel
                    {
                        Id = i,
                        ClassName = $"Class {i}",
                        StudentCount = 10 + (i % 20),
                        Description = $"This is description for class {i}"
                    });
                }
            }

            var query = ClassList.AsQueryable();

            if (!string.IsNullOrEmpty(Filter))
                query = query.Where(c => c.ClassName.Contains(Filter, StringComparison.OrdinalIgnoreCase));

            TotalPages = (int)Math.Ceiling(query.Count() / (double)PageSize);

            var paged = query.Skip((PageNumber - 1) * PageSize).Take(PageSize);

            TableData = paged.Select(c => new ClassInformationTable
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description
            }).ToList();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
                return Page();

            if (ClassInfo.Id > 0)
            {
                var existing = ClassList.FirstOrDefault(c => c.Id == ClassInfo.Id);
                if (existing != null)
                {
                    existing.ClassName = ClassInfo.ClassName;
                    existing.StudentCount = ClassInfo.StudentCount;
                    existing.Description = ClassInfo.Description;
                }
            }
            else
            {
                ClassInfo.Id = ClassList.Count > 0 ? ClassList.Max(c => c.Id) + 1 : 1;
                ClassList.Add(ClassInfo);
            }

            return RedirectToPage(new { Filter, PageNumber });
        }

        public IActionResult OnPostEdit(int id)
        {
            var item = ClassList.FirstOrDefault(c => c.Id == id);
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

        public IActionResult OnPostDelete(int id)
        {
            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null)
                ClassList.Remove(item);

            return RedirectToPage(new { Filter, PageNumber });
        }

        public IActionResult OnPostExportJson()
        {
            // Filtreli ya da tüm veriyi al
            var query = ClassList.AsQueryable();

            if (ExportType == "filtered" && !string.IsNullOrWhiteSpace(Filter))
            {
                query = query.Where(c => c.ClassName.Contains(Filter, StringComparison.OrdinalIgnoreCase));
            }

            // Veriyi tablo modeli ile eşle
            var data = query
                .Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                })
                .ToList();

            var selectedCols = SelectedColumns.Any()
                ? SelectedColumns
                : new List<string> { "ClassName", "StudentCount", "Description" };

            var json = Utils.Instance.ExportAsJson(data, selectedCols);
            var fileName = $"class_export_{DateTime.Now:yyyyMMddHHmmss}.json";

            return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", fileName);
        }
    }
}
