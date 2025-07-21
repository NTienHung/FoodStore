using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoodStoreClient.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodStoreClient.Pages.Customer.Restoran_Order_FandB
{
    public class OrderModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderModel> _logger;

        public List<Order> listOrder { get; set; } = new List<Order>();
        public List<OrderItem> listOrderItems { get; set; } = new List<OrderItem>();

        public OrderModel(IHttpClientFactory httpClientFactory, ILogger<OrderModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }
        public async Task<IActionResult> OnGet()
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Common/403");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string accId = HttpContext.Session.GetString("accId");
            if (int.TryParse(accId, out int customerId))
            {
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    // Gọi API lấy danh sách Order
                    HttpResponseMessage orderResponse = await _httpClient.GetAsync($"http://localhost:7031/api/Order/Customer/Orders/{int.Parse(accId)}");
                    if (orderResponse.IsSuccessStatusCode)
                    {
                        string orderData = await orderResponse.Content.ReadAsStringAsync();
                        using JsonDocument doc = JsonDocument.Parse(orderData);
                        if (doc.RootElement.TryGetProperty("$values", out JsonElement values))
                        {
                            listOrder = JsonSerializer.Deserialize<List<Order>>(values.GetRawText(), options) ?? new List<Order>();
                        }
                    }

                    // Gọi API lấy danh sách OrderItem
                    HttpResponseMessage itemResponse = await _httpClient.GetAsync("http://localhost:7031/api/Order/Customer/OrderItem");
                    if (itemResponse.IsSuccessStatusCode)
                    {
                        string itemData = await itemResponse.Content.ReadAsStringAsync();
                        using JsonDocument doc = JsonDocument.Parse(itemData);
                        if (doc.RootElement.TryGetProperty("$values", out JsonElement values))
                        {
                            listOrderItems = JsonSerializer.Deserialize<List<OrderItem>>(values.GetRawText(), options) ?? new List<OrderItem>();
                        }
                    }
                    return Page();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi lấy dữ liệu API: {Message}", ex.Message);
                    return StatusCode(500, "Lỗi server: " + ex.Message);
                }
            }
            else
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
        }

    }
}
