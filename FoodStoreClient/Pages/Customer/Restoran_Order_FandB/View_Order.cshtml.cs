using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoodStoreClient.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodStoreClient.Pages.Customer.Restoran_Order_FandB
{
    public class View_OrderModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderModel> _logger;

        public Order Order { get; set; } = new Order();
        public List<OrderItem> listOrderItems { get; set; } = new List<OrderItem>();
        public int? totalItem { get; set; } = 0;
        public View_OrderModel(IHttpClientFactory httpClientFactory, ILogger<OrderModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }
        public async Task<IActionResult> OnGet(string? orderId)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Common/403");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            int id = int.Parse(orderId);
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                HttpResponseMessage orderResponse = await _httpClient.GetAsync("http://localhost:7031/api/Order/Customer/Order/" + id);
                if (orderResponse.IsSuccessStatusCode)
                {
                    string orderData = await orderResponse.Content.ReadAsStringAsync();
                    Order = JsonSerializer.Deserialize<Order>(orderData, options) ?? new Order();

                }
                HttpResponseMessage itemResponse = await _httpClient.GetAsync("http://localhost:7031/api/Order/Customer/OrderItems/" + id);
                if (itemResponse.IsSuccessStatusCode)
                {
                    string itemData = await itemResponse.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(itemData);
                    if (doc.RootElement.TryGetProperty("$values", out JsonElement values))
                    {
                        listOrderItems = JsonSerializer.Deserialize<List<OrderItem>>(values.GetRawText(), options) ?? new List<OrderItem>();
                        foreach (OrderItem item in listOrderItems)
                        {
                            totalItem += item.Quantity;
                        }
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
        public async Task<IActionResult> OnPost(string? orderId, string? statusOrder)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Common/403");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string accId = HttpContext.Session.GetString("accId");
            if (string.IsNullOrEmpty(accId))
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            HttpResponseMessage orderResponse = await _httpClient.GetAsync($"http://localhost:7031/api/Order/Customer/Order/" + orderId);
            if (orderResponse.IsSuccessStatusCode)
            {
                string orderData = await orderResponse.Content.ReadAsStringAsync();
                Order = JsonSerializer.Deserialize<Order>(orderData, options) ?? new Order();
            }

            HttpResponseMessage updateResponse = await _httpClient.PutAsync(
                $"http://localhost:7031/api/Order/Customer/Status/{orderId}?status={statusOrder}",
                null
            );

            if (!updateResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to update order status.");
                return Page();
            }

            orderResponse = await _httpClient.GetAsync($"http://localhost:7031/api/Order/Customer/Order/" + orderId);
            if (orderResponse.IsSuccessStatusCode)
            {
                string orderData = await orderResponse.Content.ReadAsStringAsync();
                Order = JsonSerializer.Deserialize<Order>(orderData, options) ?? new Order();
            }

            HttpResponseMessage itemResponse = await _httpClient.GetAsync($"http://localhost:7031/api/Order/Customer/OrderItems/{orderId}");
            if (itemResponse.IsSuccessStatusCode)
            {
                string itemData = await itemResponse.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(itemData);
                if (doc.RootElement.TryGetProperty("$values", out JsonElement values))
                {
                    listOrderItems = JsonSerializer.Deserialize<List<OrderItem>>(values.GetRawText(), options) ?? new List<OrderItem>();
                    totalItem = listOrderItems.Sum(item => item.Quantity);
                }
            }

            return Page();
        }

    }
}
