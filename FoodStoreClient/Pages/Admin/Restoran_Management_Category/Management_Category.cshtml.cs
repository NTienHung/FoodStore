using FoodStoreAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodStoreClient.Pages.Admin.Restoran_Management_Category
{
    public class Management_CategoryModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string AccountApiUrl = "http://localhost:7031/api/Categorys/Admin/Categorys";
        public List<CategoryDTO> listCate = new List<CategoryDTO>();
        public decimal DiscountPercentage { get; set; } = 0.1m;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;
        public string status { get; set; } = "";
        public string searchCate { get; set; } = "";
        public string sortCate { get; set; } = "";
        public Management_CategoryModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<IActionResult> OnGet(int? pageNumber, string? search, string? sort)
        {
            string accId = HttpContext.Session.GetString("accId");
            if (accId == null)
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
            int total = 0;
            searchCate = search;
            sortCate = sort;
            CurrentPage = pageNumber ?? 10;
            try
            {
                string requestUrl;

                if (!string.IsNullOrWhiteSpace(searchCate) && !string.IsNullOrEmpty("sort"))
                {
                    requestUrl = $"{AccountApiUrl}?search={searchCate}&sort={sort}";
                } else if (!string.IsNullOrWhiteSpace(searchCate))
                {
                    requestUrl = $"{AccountApiUrl}?search={searchCate}";
                } else if(!string.IsNullOrEmpty("sort"))
                {
                    requestUrl = $"{AccountApiUrl}?sort={sort}";
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
                listCate = JsonSerializer.Deserialize<List<CategoryDTO>>(valuesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<CategoryDTO>();
                total = listCate.Count;
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
    }
}
