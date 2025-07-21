using FoodStoreClient.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodStoreClient.Pages.Ship.Restoran_Management_Order
{
    public class Management_OrderModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string OrderForShipApiUrl = "http://localhost:7031/api/Order/Ship/OderForShip";
        private readonly string OrderForShhipFilterApiUrl = "http://localhost:7031/api/Order/Ship/OrderForShipFilter";
        private readonly string OrderForShipFilterAndSearchApiUrl = "http://localhost:7031/api/Order/Ship/OrderForShipFilterAndSearch";
        public List<OrderDTO> listOrder = new List<OrderDTO>();
        public string statusOrder { get; set; } = "";
        public string searchOrder { get; set; } = "";
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public Management_OrderModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<IActionResult> OnGet(int? pageNumber, string? status, string? search)
        {
            string accId = HttpContext.Session.GetString("accId");
            if (accId == null)
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }

            statusOrder = status;
            searchOrder = search;
            int totalOrders = 0;
            CurrentPage = pageNumber ?? 1;

            try
            {
                string requestUrl;

                if (!string.IsNullOrWhiteSpace(searchOrder))
                {
                    requestUrl = $"{OrderForShipFilterAndSearchApiUrl}?searchOrder={searchOrder}";
                }
                else if (!string.IsNullOrWhiteSpace(statusOrder) && statusOrder != "-1")
                {
                    requestUrl = $"{OrderForShhipFilterApiUrl}?statusOrder={statusOrder}";
                }
                else
                {
                    requestUrl = OrderForShipApiUrl;
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
                listOrder = JsonSerializer.Deserialize<List<OrderDTO>>(valuesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrderDTO>();
                totalOrders = listOrder.Count;
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

            TotalPages = (int)Math.Ceiling(totalOrders / (double)PageSize);
            return Page();
        }
    }
}
