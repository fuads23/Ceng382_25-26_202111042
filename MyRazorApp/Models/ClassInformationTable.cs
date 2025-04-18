namespace MyRazorApp.Models
{
    public class ClassInformationTable
    {
        public int Id { get; set; }  // arka planda işlemler için
        public string ClassName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
