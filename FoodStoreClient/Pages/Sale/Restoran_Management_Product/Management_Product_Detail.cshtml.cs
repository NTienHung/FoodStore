using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoodStoreClient.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FoodStoreClient.Pages.Sale.Restoran_Management_Product
{
    public class Management_Product_DetailModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string ProductApiUrl = "http://localhost:7031/api/Product/Sale/Product";
        
        public Management_Product_DetailModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public Product Product { get; set; }
        public async Task<IActionResult> OnGet(string id)
        {
            string accId = HttpContext.Session.GetString("accId");
            if (accId != null)
            {
                var requestUrl = $"{ProductApiUrl}?proId={int.Parse(id)}";
                var response = await client.GetAsync(requestUrl);
                var responseBody = await response.Content.ReadAsStringAsync();
                Product = JsonSerializer.Deserialize<Product>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return Page();
            }
            else
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
        }
    }
}
