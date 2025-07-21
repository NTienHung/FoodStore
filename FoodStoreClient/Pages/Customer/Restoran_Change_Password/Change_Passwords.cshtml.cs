using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoodStoreClient.DTO;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FoodStoreClient.Pages.Customer.Restoran_Change_Password
{
    public class Change_PasswordsModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string ProfileApiUrl = "http://localhost:7031/api/Account/Customer/Account/Profile";
        private readonly string ChangePassApiUrl = "http://localhost:7031/api/Account/Guest/Account/ChangePassword";
        public AccountDTO? account { get; set; }
        public Change_PasswordsModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public IActionResult OnGet()
        {
            string accId = HttpContext.Session.GetString("accId");
            if (accId == null)
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
            return Page();
        }
        public async Task<IActionResult> OnPost(IFormCollection form)
        {
            string old_password = form["old_password"];
            string password = form["password"];
            string re_password = form["re_password"];
            string accId = HttpContext.Session.GetString("accId");

            var requestUrl = $"{ProfileApiUrl}/{int.Parse(accId)}";
            var response = await client.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Account not found!";
                return RedirectToPage("/Customer/Restoran_Change_Password/Change_Passwords");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            account = JsonSerializer.Deserialize<AccountDTO>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!int.TryParse(accId, out int accountId))
            {
                TempData["ErrorMessage"] = "Invalid account!";
                return RedirectToPage("/Customer/Restoran_Change_Password/Change_Passwords");
            }

            string hashedOldPassword = HashPasswordSHA256(old_password);
            if (hashedOldPassword.Equals(account.Password))
            {
                if (password.Equals(re_password))
                {
                    if (password.Length >= 8 && Regex.IsMatch(password, @"^[A-Z]") && Regex.IsMatch(password, @"\d") && Regex.IsMatch(password, @"[!@#$%^&*(),.?:{}|<>]"))
                    {
                        string hashedPassword = HashPasswordSHA256(password);
                        account.Password = hashedPassword;
                        account.UpdateAt = DateTime.Now;
                        var requestCheckUrl = $"{ChangePassApiUrl}?email={account.Email}&pass={password}";
                        var responseCheck = await client.PutAsync(requestCheckUrl, null);

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
                        TempData["ErrorMessage"] = "Password update successful!";
                        return RedirectToPage("/Guest/Restoran_Login/Login");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Password must be at least 8 characters, start with an uppercase letter, contain at least one number, and one special character.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Password and confirmation do not match!";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Old password is incorrect!";
            }
            TempData["Old_password"] = old_password;
            TempData["Password"] = password;
            TempData["Re_Password"] = re_password;
            return RedirectToPage("/Customer/Restoran_Change_Password/Change_Passwords");
        }
        private string HashPasswordSHA256(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
