using Microsoft.Data.SqlClient;

namespace MyRazorApp.Helpers
{
    public static class DataSeeder
    {
        public static void SeedInitialData(IConfiguration config)
        {
            using var conn = new SqlConnection(config.GetConnectionString("SchoolDbConnection"));
            conn.Open();

            var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Classes", conn);
            int count = (int)checkCmd.ExecuteScalar();

            if (count < 100)
            {
                for (int i = 1; i <= 100; i++)
                {
                    var cmd = new SqlCommand(@"
                        INSERT INTO Classes (ClassName, StudentCount, Description, IsActive)
                        VALUES (@ClassName, @StudentCount, @Description, 1)", conn);

                    cmd.Parameters.AddWithValue("@ClassName", $"Class {i}");
                    cmd.Parameters.AddWithValue("@StudentCount", 10 + i);
                    cmd.Parameters.AddWithValue("@Description", $"Auto-generated class {i}");

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
