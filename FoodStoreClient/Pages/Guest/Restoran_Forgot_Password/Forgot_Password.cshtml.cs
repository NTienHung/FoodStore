using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoodStoreClient.Models;
using FoodStoreClient.Pages.Guest.Verify_Email;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FoodStoreClient.Pages.Guest.Restoran_Forgot_Password
{
    public class Forgot_PasswordModel : PageModel
    {
        private readonly HttpClient _client;
        private readonly string CheckExistApiUrl = "http://localhost:7031/api/Account/Guest/Account/ForgotPassword";
        public Account account;
        private readonly ForgotPasswordService _emailService;
        public Forgot_PasswordModel(HttpClient client, ForgotPasswordService emailService)
        {
            _client = client;
            _emailService = emailService;
        }
        public void OnGet()
        {
        }

        [BindProperty]
        public string Email { get; set; }
        public async Task<IActionResult> OnPost(IFormCollection form)
        {
            string email = form["email"];
            var requestCheckUrl = $"{CheckExistApiUrl}?email={email}";
            var responseCheck = await _client.GetAsync(requestCheckUrl);
            var responseBodyCheck = await responseCheck.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(responseBodyCheck) && Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                var acc = JsonSerializer.Deserialize<Account>(responseBodyCheck, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                await _emailService.SendVerificationEmailAsync(Email, acc.Name);
                TempData["ErrorMessage"] = "Email exists, we need email verification, please check your email box!";
            }
            else
            {
                TempData["ErrorMessage"] = "Email does not exist or email invalid, please enter correct email for verification!";
            }
            return RedirectToPage("/Guest/Restoran_Forgot_Password/Forgot_Password");
        }
    }
}
