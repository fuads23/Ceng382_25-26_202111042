using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace MyRazorApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _config;

        public LoginModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public string Username { get; set; }= string.Empty;

        [BindProperty]
        public string Password { get; set; }= string.Empty;

        public string ErrorMessage { get; set; } = "";

        public IActionResult OnPost()
        {
            using var conn = new SqlConnection(_config.GetConnectionString("SchoolDbConnection"));
            conn.Open();

            var cmd = new SqlCommand("SELECT * FROM Users WHERE Username = @Username AND Password = @Password", conn);
            cmd.Parameters.AddWithValue("@Username", Username);
            cmd.Parameters.AddWithValue("@Password", Password); // Şifre hashlenmiyorsa düz kontrol

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string token = Guid.NewGuid().ToString();
                string sessionId = Guid.NewGuid().ToString();

                HttpContext.Session.SetString("username", Username);
                HttpContext.Session.SetString("token", token);
                HttpContext.Session.SetString("session_id", sessionId);

                Response.Cookies.Append("username", Username);
                Response.Cookies.Append("token", token);
                Response.Cookies.Append("session_id", sessionId);

                reader.Close();

                // Token ve sessionId'yi DB'ye kaydet
                var updateCmd = new SqlCommand("UPDATE Users SET Token = @Token, SessionId = @SessionId WHERE Username = @Username", conn);
                updateCmd.Parameters.AddWithValue("@Token", token);
                updateCmd.Parameters.AddWithValue("@SessionId", sessionId);
                updateCmd.Parameters.AddWithValue("@Username", Username);
                updateCmd.ExecuteNonQuery();

                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }
        }
    }
}
