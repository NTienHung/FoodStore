using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodStoreClient.Pages.Admin.Restoran_Management_Product
{
    public class Management_ProductModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string GetProductApiUrl = "http://localhost:7031/api/admin/product";

        public string errorMessage { get; set; } = "";
        public DateTime? dateNow { get; set; } = DateTime.Now;
        public DateTime? datePast { get; set; } = DateTime.Now.AddYears(-2);
        public DateTime? dateSevenDay { get; set; } = new DateTime(2024, 4, 1);

        public List<Product> Products { get; set; } = new List<Product>();
        public List<Product> FilteredProducts { get; set; } = new List<Product>();

        public Management_ProductModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IActionResult> OnGet()
        {
            string accId = HttpContext.Session.GetString("accId");
            if (accId == null)
                return RedirectToPage("/Guest/Restoran_Login/Login");

            await LoadAllProducts();
            FilteredProducts = FilterProducts(dateSevenDay, dateNow);
            return Page();
        }

        public async Task<IActionResult> OnPost(string? from, string? to)
        {
            string accId = HttpContext.Session.GetString("accId");
            if (accId == null)
                return RedirectToPage("/Guest/Restoran_Login/Login");

            if (!DateTime.TryParse(from, out DateTime fromDate) || !DateTime.TryParse(to, out DateTime toDate))
            {
                errorMessage = "Invalid date range.";
                return Page();
            }

            await LoadAllProducts();
            FilteredProducts = FilterProducts(fromDate, toDate);
            dateSevenDay = fromDate;
            dateNow = toDate;

            return Page();
        }

        private async Task LoadAllProducts()
        {
            try
            {
                var response = await client.GetAsync(GetProductApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(responseBody);
                    var valuesJson = jsonDoc.RootElement.GetProperty("$values").ToString();
                    Products = JsonSerializer.Deserialize<List<Product>>(valuesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Product>();
                    Console.WriteLine($">>> Products loaded from API: {Products.Count}");
                }
                else
                {
                    errorMessage = $"API Error: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Exception: {ex.Message}";
            }
        }

        private List<Product> FilterProducts(DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate == null || toDate == null)
                return Products;

            return Products
                .Where(p => p.CreateAt.HasValue && p.CreateAt.Value.Date >= fromDate.Value.Date && p.CreateAt.Value.Date <= toDate.Value.Date)
                .ToList();
        }
    }

    // Định nghĩa Product DTO (Model)
    public class Product
    {
        public int ProId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public string Unit { get; set; }
        public string Images { get; set; }
        public int Quantity { get; set; }
        public string ProductStatus { get; set; }
        public int CateId { get; set; }
        public string CateName { get; set; }
        public DateTime? CreateAt { get; set; }
        public int AccId { get; set; }
        public string NameAccount { get; set; }
    }
}
