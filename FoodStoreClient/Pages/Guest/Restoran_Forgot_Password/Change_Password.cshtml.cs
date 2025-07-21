using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FoodStoreClient.Pages.Guest.Restoran_Forgot_Password
{
    public class Change_PasswordModel : PageModel
    {
        private readonly HttpClient _client;
        private readonly string ChangePassApiUrl = "http://localhost:7031/api/Account/Guest/Account/ChangePassword";
        public string Email { get; set; }
        public Change_PasswordModel(HttpClient client)
        {
            _client = client;
        }

        public IActionResult OnGet(IFormCollection form)
        {
            Email = Request.Query["email"];
            if (string.IsNullOrEmpty(Email))
            {
                TempData["ErrorMessage"] = "Invalid email!";
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
            return Page();
        }

        public async Task<IActionResult> OnPost(IFormCollection form)
        {
            string password = form["password"];
            string re_password = form["re_password"];
            string email = form["email"];

            if (!password.Equals(re_password))
            {
                TempData["ErrorMessage"] = "Password does not match the re-password!";
                return Page();
            }

            if (password.Length <= 8 ||
                !Regex.IsMatch(password, @"^[A-Z]") ||
                !Regex.IsMatch(password, @"\d") ||
                !Regex.IsMatch(password, @"[!@#$%^&*(),.?:{}|<>]"))
            {
                TempData["ErrorMessage"] = "Password must be at least 8 digits, start with an uppercase letter, contain at least one number, and one special character.";
                return Page();
            }

            var requestCheckUrl = $"{ChangePassApiUrl}?email={email}&pass={password}";
            var responseCheck = await _client.PutAsync(requestCheckUrl, null);

            if (responseCheck.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Password update successful!";
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
            else
            {
                var errorResponse = await responseCheck.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Password update failed: {errorResponse}";
            }

            return Page();
        }
    }
}
