using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using System.Text.Json;

namespace MyRazorApp.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty] public string Username { get; set; } = string.Empty;
        [BindProperty] public string Password { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.json");
            var usersJson = System.IO.File.ReadAllText(filePath);
            var users = JsonSerializer.Deserialize<List<User>>(usersJson);

            var user = users?.FirstOrDefault(u => u.Username == Username && u.Password == Password && u.IsActive);

            if (user != null)
            {
                var token = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("username", user.Username);
                HttpContext.Session.SetString("token", token);
                HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };

                Response.Cookies.Append("username", user.Username, cookieOptions);
                Response.Cookies.Append("token", token, cookieOptions);
                Response.Cookies.Append("session_id", HttpContext.Session.Id, cookieOptions);

                return RedirectToPage("/Index");
            }

            ErrorMessage = "Username or password is incorrect.";
            return Page();
        }
    }
}