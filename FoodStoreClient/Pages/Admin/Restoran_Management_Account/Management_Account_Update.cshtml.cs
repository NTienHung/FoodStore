using FoodStoreClient.DTO;
using FoodStoreClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodStoreClient.Pages.Admin.Restoran_Management_Account
{
    public class Management_Account_UpdateModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string ProfileApiUrl = "http://localhost:7031/api/Account/Customer/Account/Profile";
        private readonly string UpdateApiUrl = "http://localhost:7031/api/Account/Admin/Update/Account";
        private readonly FoodStoreAPI.Models.FoodStoreContext foodStoreContext = new FoodStoreAPI.Models.FoodStoreContext();

        private int accountID { get; set; } = 0;

        public List<RoleDTO> listRole = new List<RoleDTO>();
        [BindProperty]
        public AccountDTO account { get; set; } = new AccountDTO();
        public Management_Account_UpdateModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<ActionResult> OnGet(string accountId)
        {
            listRole = foodStoreContext.Roles.Where(x => x.RoleId != 1).Select(x => new RoleDTO
            {
                RoleId = x.RoleId,
                RoleName = x.RoleName
            }).ToList();
            string? accId = HttpContext.Session.GetString("accId");
            accountID = int.Parse(accountId);
            if (string.IsNullOrEmpty(accId))
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }

            try
            {
                var requestUrl = $"{ProfileApiUrl}/{int.Parse(accountId)}";
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
                else
                {
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

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                //using var httpClient = new HttpClient();
                //var apiUrl = $"http://localhost:7031/Account/api/Admin/Update/Account?accId={account.AccId}&roleId={account.RoleId}";

                var requestCheckUrl = $"{UpdateApiUrl}?accId={account.AccId}&roleId={account.RoleId}";
                var responseCheck = await client.PutAsync(requestCheckUrl, null);

                if (responseCheck.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Cập nhật tài khoản thành công!";
                    return RedirectToPage("/Admin/Restoran_Management_Account/Management_Account"); // về lại trang danh sách
                }
                else
                {
                    var errorContent = await responseCheck.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = "Cập nhật thất bại: " + errorContent;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi trong quá trình cập nhật!";
                return Page();
            }
        }
    }
}
