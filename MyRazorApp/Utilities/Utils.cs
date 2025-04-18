using System.Text.Json;

namespace MyRazorApp.Helpers
{
    public class Utils
    {
        private static Utils _instance = new Utils();
        public static Utils Instance => _instance;

        private Utils() { }

        public string ExportAsJson<T>(List<T> data, List<string> selectedColumns)
        {
            var filtered = data.Select(item =>
            {
                var obj = new Dictionary<string, object?>();
                foreach (var prop in typeof(T).GetProperties())
                {
                    if (selectedColumns.Contains(prop.Name))
                    {
                        obj[prop.Name] = prop.GetValue(item);
                    }
                }
                return obj;
            }).ToList();

            return JsonSerializer.Serialize(filtered, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
