using FoodStoreClient.DTO;
using FoodStoreClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FoodStoreClient.Pages.Ship.Restoran_Management_Order
{
    public class Management_Order_ViewModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string ListOrderItemApiUrl = "http://localhost:7031/api/Order/Ship/OrderItem";
        private readonly string OrderDetailApiUrl = "http://localhost:7031/api/Order/Ship/OrderDetail";
        private readonly string OrderUpdateApiUrl = "http://localhost:7031/api/Order/Ship/Status";
        public OrderDTO Order { get; set; } = new OrderDTO();
        public List<OrderItem> listOrderItem { get; set; } = new List<OrderItem>();
        public int? totalItem { get; set; } = 0;
        public Management_Order_ViewModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<IActionResult> OnGet(string? orderId)
        {
            string accId = HttpContext.Session.GetString("accId");
            if (accId != null)
            {
                string requestOrderDetailUrl = $"{OrderDetailApiUrl}?orderId={orderId}";
                var responseOrderDetail = await client.GetAsync(requestOrderDetailUrl);
                if (!responseOrderDetail.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"API Error: {responseOrderDetail.StatusCode}";
                    return Page();
                }
                var responseOrderDetailBody = await responseOrderDetail.Content.ReadAsStringAsync();
                Order = JsonSerializer.Deserialize<DTO.OrderDTO>(responseOrderDetailBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                string requestOrderItemslUrl = $"{ListOrderItemApiUrl}?orderId={orderId}";
                var responseOrderItems = await client.GetAsync(requestOrderItemslUrl);
                if (!responseOrderItems.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"API Error: {responseOrderItems.StatusCode}";
                    return Page();
                }
                var responseOrderItemsBody = await responseOrderItems.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(responseOrderItemsBody);
                var valuesJson = jsonDoc.RootElement.GetProperty("$values").ToString();
                listOrderItem = JsonSerializer.Deserialize<List<OrderItem>>(valuesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrderItem>();
                foreach (OrderItem item in listOrderItem)
                {
                    totalItem += item.Quantity;
                }
                return Page();
            }
            else
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
        }
        public async Task<IActionResult> OnPost(string? orderId, string? statusOrder)
        {
            string accId = HttpContext.Session.GetString("accId");
            if (accId != null)
            {
                string requestOrderDetailUrl = $"{OrderDetailApiUrl}?orderId={orderId}";
                var responseOrderDetail = await client.GetAsync(requestOrderDetailUrl);
                if (!responseOrderDetail.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"API Error: {responseOrderDetail.StatusCode}";
                    return Page();
                }
                var responseOrderDetailBody = await responseOrderDetail.Content.ReadAsStringAsync();
                Order = JsonSerializer.Deserialize<OrderDTO>(responseOrderDetailBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var content = new StringContent(JsonSerializer.Serialize(Order), Encoding.UTF8, "application/json");
                var requestUpdateUrl = $"{OrderUpdateApiUrl}/{Order.OrderId}?status={statusOrder}";
                var responseUpdate = await client.PutAsync(requestUpdateUrl, content);
                
                Order = JsonSerializer.Deserialize<OrderDTO>(responseOrderDetailBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                string requestOrderItemslUrl = $"{ListOrderItemApiUrl}?orderId={orderId}";
                var responseOrderItems = await client.GetAsync(requestOrderItemslUrl);
                if (!responseOrderItems.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"API Error: {responseOrderItems.StatusCode}";
                    return Page();
                }
                var responseOrderItemsBody = await responseOrderItems.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(responseOrderItemsBody);
                var valuesJson = jsonDoc.RootElement.GetProperty("$values").ToString();
                listOrderItem = JsonSerializer.Deserialize<List<OrderItem>>(valuesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrderItem>();
                foreach (OrderItem item in listOrderItem)
                {
                    totalItem += item.Quantity;
                }
                return Redirect("/Ship/Restoran_Management_Order/Management_Order_View?orderId=" + Order.OrderId);
            }
            else
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
        }
    }
}
