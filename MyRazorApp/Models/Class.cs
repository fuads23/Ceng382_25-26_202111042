using System.ComponentModel.DataAnnotations;
namespace MyRazorApp.Models
{
 public class Class
 {
 [Key]
 public int Id { get; set; }
 [Required]
 //public string? Name { get; set; } // string-> string?
 public string Name { get; set; } = string.Empty;
 [Required]
 
 public int PersonCount { get; set; }
// public string? Description { get; set; } // string-> string?
public string Description { get; set; } = string.Empty;
 [Required]
 public bool IsActive { get; set; }
 }
}