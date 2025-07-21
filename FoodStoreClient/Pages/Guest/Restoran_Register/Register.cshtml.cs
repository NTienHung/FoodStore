using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoodStoreClient.Models;
using FoodStoreClient.Pages.Guest.Verify_Email;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text;

namespace FoodStoreClient.Pages.Guest.Restoran_Register
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _client;
        private readonly string CheckExistApiUrl = "http://localhost:7031/api/Account/Guest/Account/ExistAccount";
        private readonly string RegisterApiUrl = "http://localhost:7031/api/Account/Guest/Account/Register";
        public Account account;
        public RegisterModel(HttpClient client, EmailService emailService)
        {
            _client = client;
            _emailService = emailService;
        }

        private readonly EmailService _emailService;
        public void OnGet()
        {

        }

        [BindProperty]
        public string Email { get; set; }
        public async Task<IActionResult> OnPost(IFormCollection form)
        {
            string username = form["username"];
            string name = form["name"];
            string password = form["password"];
            string phone = form["phone"];
            string re_password = form["re_password"];
            string email = form["email"];
            string address = form["address"];
            DateTime datenow = DateTime.Now;

            var requestCheckUrl = $"{CheckExistApiUrl}?username={username}&email={email}";
            var responseCheck = await _client.GetAsync(requestCheckUrl);
            var responseBodyCheck = await responseCheck.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(responseBodyCheck))
            {
                if (!username.Contains(" "))
                {
                    if (password.Length > 8 && Regex.IsMatch(password, @"^[A-Z]") && Regex.IsMatch(password, @"\d") && Regex.IsMatch(password, @"[!@#$%^&*(),.?:{}|<>]"))
                    {
                        if (!password.Equals(re_password))
                        {
                            TempData["ErrorMessage"] = "Password does not match the re-password!";
                        }
                        else if (phone.Length != 10 && !Regex.IsMatch(phone, @"^(03|05|07|08|09)\d{8}$"))
                        {
                            TempData["ErrorMessage"] = "Phone number must be 10 digits, and have a matching prefix!";
                        }
                        else if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                        {
                            TempData["ErrorMessage"] = "Invalid email or email early exist!";
                        }
                        else
                        {
                            var accNew = new Account
                            {
                                Email = email,
                                Phone = phone,
                                Name = name,
                                Username = username,
                                Password = password,
                                CreateAt = datenow,
                                RoleId = 3,
                                Address = address,
                            };
                            var content = new StringContent(JsonSerializer.Serialize(accNew), Encoding.UTF8, "application/json");
                            var responseAddNew = await _client.PostAsync(RegisterApiUrl, content);
                            if (!responseAddNew.IsSuccessStatusCode)
                            {
                                TempData["ErrorMessage"] = $"API Register fail: {responseAddNew.StatusCode}";
                                return RedirectToPage("/Guest/Restoran_Register/Register");
                            }
                            var responseBody = await responseAddNew.Content.ReadAsStringAsync();
                            TempData["ErrorMessage"] = "Create new account successfuly!";
                            await _emailService.SendVerificationEmailAsync(Email, name);
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Password must be at least 8 digits, must have an uppercase letter at the beginning of the string, must have at least one number, and must have at least one special character.!";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Username cannot contain spaces!";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "The username early exist!";
            }
            TempData["Username"] = username;
            TempData["Password"] = password;
            TempData["Name"] = name;
            TempData["Phone"] = phone;
            TempData["Re_password"] = re_password;
            TempData["Email"] = email;
            TempData["Address"] = address;
            return RedirectToPage("/Guest/Restoran_Register/Register");
        }
    }
}
