using FoodStoreAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodStoreClient.Pages.Admin.Restoran_Management_Category
{
    public class Management_Category_CreateModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string AccountApiUrl = "http://localhost:7031/api/Categorys/Admin/Categorys";
        public List<CategoryDTO> listCate = new List<CategoryDTO>();

        [BindProperty]
        public CategoryDTO CategoryDTO { get; set; } = new CategoryDTO();
        public Management_Category_CreateModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<IActionResult> OnGet()
        {
            string accId = HttpContext.Session.GetString("accId");
            if (accId == null)
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
            try
            {
                string requestUrl;
                requestUrl = AccountApiUrl;
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
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync("http://localhost:7031/api/Categorys/Admin/AddCategory", CategoryDTO);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Admin/Restoran_Management_Category/Management_Category_Create");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = error;
                return Page();
            }
        }
    }
}
