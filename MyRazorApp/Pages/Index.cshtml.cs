using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>();

        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new ClassInformationModel();

        [BindProperty]
        public bool IsEditing { get; set; } = false;

        public void OnGet()
        {
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (ClassInfo.Id > 0)
            {
                // Eğer ID varsa, mevcut kaydı güncelle
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
                // Yeni bir kayıt ekle
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
        public string ClassName { get; set; } = string.Empty; // Varsayılan boş değer
        public int StudentCount { get; set; }
        public string Description { get; set; } = string.Empty; // Varsayılan boş değer
    }
}