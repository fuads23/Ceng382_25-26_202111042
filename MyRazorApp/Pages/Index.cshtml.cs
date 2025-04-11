using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>();

        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new ClassInformationModel();

        public List<ClassInformationTable> ClassInformationTableList { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Filter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public IActionResult OnGet(int? pageNumber, string? filter)
        {
            // --- Sınıf listesi hazırlanıyor ---
            if (!ClassList.Any())
            {
                for (int i = 1; i <= 100; i++)
                {
                    ClassList.Add(new ClassInformationModel
                    {
                        Id = i,
                        ClassName = $"Class {i}",
                        StudentCount = i * 5,
                        Description = $"Description for Class {i}"
                    });
                }
            }

            // --- Filtreleme işlemi ---
            Filter = filter;
            var filtered = string.IsNullOrEmpty(filter)
                ? ClassList
                : ClassList.Where(c => c.ClassName.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();

            // --- Sayfalama işlemi ---
            int pageSize = 10;
            CurrentPage = pageNumber ?? 1;
            TotalPages = (int)Math.Ceiling(filtered.Count / (double)pageSize);

            ClassInformationTableList = filtered
                .Skip((CurrentPage - 1) * pageSize)
                .Take(pageSize)
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
            {
                return Page();
            }

            if (ClassInfo.Id > 0)
            {
                var existingClass = ClassList.FirstOrDefault(c => c.Id == ClassInfo.Id);
                if (existingClass != null)
                {
                    existingClass.ClassName = ClassInfo.ClassName;
                    existingClass.StudentCount = ClassInfo.StudentCount;
                    existingClass.Description = ClassInfo.Description;
                }
            }
            else
            {
                ClassInfo.Id = ClassList.Count > 0 ? ClassList.Max(c => c.Id) + 1 : 1;
                ClassList.Add(ClassInfo);
            }

            return RedirectToPage();
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

            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                ClassList.Remove(item);
            }

            return RedirectToPage();
        }
    }

    public class ClassInformationModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Class name is required.")]
        public string ClassName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Student count is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Student count must be at least 1.")]
        public int StudentCount { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;
    }
}