using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; } // Auto-incremented

        [Required(ErrorMessage = "Class name is required.")]
        public string ClassName { get; set; } = string.Empty; // Varsayılan boş değer

        [Required(ErrorMessage = "Student count is required.")]
        [Range(1, 1000, ErrorMessage = "Student count must be between 1 and 1000.")]
        public int StudentCount { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty; // Varsayılan boş değer
    }
}