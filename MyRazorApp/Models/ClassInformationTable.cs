namespace MyRazorApp.Models
{
    public class ClassInformationTable
    {
        public int Id { get; set; } // Arka planda işlem için kullanılacak
        public string ClassName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}