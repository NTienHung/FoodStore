using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoodStoreClient.DTO;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodStoreClient.Pages.Admin.Restoran_Management_Account
{
    public class Management_AccountModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string AccountApiUrl = "http://localhost:7031/api/Account/Admin/Accounts";
        private readonly string AccountSearchApiUrl = "http://localhost:7031/api/Account/Admin/Account/Search";
        public List<AccountDTO> listAcc = new List<AccountDTO>();
        public decimal DiscountPercentage { get; set; } = 0.1m;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;
        public string status { get; set; } = "";
        public string searchAccount { get; set; } = "";
        public string sort { get; set; } = "";
        public Management_AccountModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<IActionResult> OnGet(int? pageNumber, string? search, string? sortBy)
        {
            string accId = HttpContext.Session.GetString("accId");
            if (accId == null)
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
            int total = 0;
            searchAccount = search;
            sort = sortBy;
            CurrentPage = pageNumber ?? 10;
            try
            {
                string requestUrl;

                if (!string.IsNullOrWhiteSpace(searchAccount) || !string.IsNullOrWhiteSpace(sort))
                {
                    requestUrl = $"{AccountSearchApiUrl}?searchAccount={searchAccount}&sort={sort}";
                }
                else
                {
                    requestUrl = AccountApiUrl;
                }

                var response = await client.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"API Error: {response.StatusCode}";
                    return Page();
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(responseBody) || responseBody == "null")
                {
                    TempData["ErrorMessage"] = "API trả về dữ liệu rỗng.";
                    return Page();
                }
                var jsonDoc = JsonDocument.Parse(responseBody);
                var valuesJson = jsonDoc.RootElement.GetProperty("$values").ToString();
                listAcc = JsonSerializer.Deserialize<List<AccountDTO>>(valuesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AccountDTO>();
                total = listAcc.Count;
            }
            catch (JsonException ex)
            {
                TempData["ErrorMessage"] = "Lỗi parse JSON từ API: " + ex.Message;
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi hệ thống: " + ex.Message;
                return Page();
            }

            TotalPages = (int)Math.Ceiling(total / (double)PageSize);
            return Page();
        }

        public async Task<IActionResult> OnGetBanAsync(string accountId)
        {
            try
            {
                int accId = int.Parse(accountId);
                Console.Write(accId);
                //using var httpClient = new HttpClient();
                var UpdateApiUrl = "http://localhost:7031/api/Account/Admin/Update/Account";

                var requestCheckUrl = $"{UpdateApiUrl}?accId={accId}&roleId=5";
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
        public async Task<IActionResult> OnGetUnbanAsync(string accountId)
        {
            try
            {
                int accId = int.Parse(accountId);
                //using var httpClient = new HttpClient();
                var UpdateApiUrl = "http://localhost:7031/api/Account/Admin/Update/Account";

                var requestCheckUrl = $"{UpdateApiUrl}?accId={accId}&roleId=3";
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
