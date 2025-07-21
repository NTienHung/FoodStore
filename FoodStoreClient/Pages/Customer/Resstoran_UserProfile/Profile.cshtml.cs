using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoodStoreClient.Models;
using FoodStoreClient.DTO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FoodStoreClient.Pages.Customer.Resstoran_UserProfile
{
    public class ProfileModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string ProfileApiUrl = "http://localhost:7031/api/Account/Customer/Account/Profile";
        private readonly string ProfileUpdateApiUrl = "http://localhost:7031/api/Account/Customer/Account/Update/Information";

        public AccountDTO? account { get; set; } = new AccountDTO();

        public ProfileModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IActionResult> OnGet()
        {
            string? accId = HttpContext.Session.GetString("accId");
            if (string.IsNullOrEmpty(accId))
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }

            try
            {
                var requestUrl = $"{ProfileApiUrl}/{int.Parse(accId)}";
                var response = await client.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to fetch profile information!";
                    return Page();
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                account = JsonSerializer.Deserialize<AccountDTO>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (account == null)
                {
                    TempData["ErrorMessage"] = "No account data found!";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching profile data.";
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPost(IFormCollection form)
        {
            string? accId = HttpContext.Session.GetString("accId");
            if (string.IsNullOrEmpty(accId))
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
            else
            {
                string phone = form["phone"];
                string name = form["name"];
                string address = form["address"];

                if (phone.Length != 10 || !Regex.IsMatch(phone, @"^(03|05|07|08|09)\d{8}$"))
                {
                    TempData["ErrorMessage"] = "Invalid phone number!";
                }
                else if (address.Length <= 20)
                {
                    TempData["ErrorMessage"] = "Address must be complete and longer than 20 characters!";
                }
                else
                {
                    try
                    {
                        var accountUpdate = new AccountDTO
                        {
                            Phone = phone,
                            Address = address,
                            Name = name,
                            UpdateAt = DateTime.Now
                        };

                        var content = new StringContent(JsonSerializer.Serialize(accountUpdate), Encoding.UTF8, "application/json");
                        var requestUpdateUrl = $"{ProfileUpdateApiUrl}?id={accId}";
                        var responseUpdate = await client.PutAsync(requestUpdateUrl, content);

                        if (!responseUpdate.IsSuccessStatusCode)
                        {
                            TempData["ErrorMessage"] = "Failed to update data on API!";
                        }
                        else
                        {
                            TempData["SuccessMessage"] = "Data updated successfully!";
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "An error occurred while updating profile data.";
                    }
                }
            }
            var requestUrl = $"{ProfileApiUrl}/{int.Parse(accId)}";
            var response = await client.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to fetch profile information!";
                return Page();
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            account = JsonSerializer.Deserialize<AccountDTO>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Page();
        }

    }
}
