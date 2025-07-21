using FoodStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text;
namespace FoodStoreClient.Pages.Customer.Restoran_Order_FandB
{
    public class CartModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CartModel> _logger;

        public List<Order> listOrder { get; set; } = new List<Order>();
        public List<OrderItem> listOrderItems { get; set; } = new List<OrderItem>();
        public List<OrderItem> listOrderItemsCheck { get; set; } = new List<OrderItem>();

        private const string PendingStatus = "Pending";
        private const string AcceptStatus = "Accept Order";
        public List<Product> products { get; set; }

        public CartModel(IHttpClientFactory httpClientFactory, ILogger<CartModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            string accId = HttpContext.Session.GetString("accId");
            FoodStoreContext context = new FoodStoreContext();
            if (int.TryParse(accId, out int customerId))
            {
                listOrder = context.Orders
                    .Include(x => x.OrderItems)
                    .Where(x => x.CustomerId == int.Parse(accId) && x.Status == PendingStatus && x.Gtotal == 0)
                    .ToList();

                listOrderItems = context.OrderItems
                    .Include(x => x.Product)
                    .ToList();

                return Page();
            }
            else
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
        }

        public IActionResult OnPostCheckout(IFormCollection form)
        {
            FoodStoreContext context = new FoodStoreContext();
            string accId = HttpContext.Session.GetString("accId");
            if (string.IsNullOrEmpty(accId) || !int.TryParse(accId, out int customerId))
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }

            if (!float.TryParse(form["totalAmount"], out float totalAmount))
            {
                return Page();
            }

            var ordersToUpdate = context.Orders
                .Where(x => x.CustomerId == customerId && x.Status == PendingStatus)
                .OrderByDescending(x => x.CreateAt)
                .FirstOrDefault();
            context.SaveChanges();
            if (ordersToUpdate == null)
            {
                return Page();
            }

            ordersToUpdate.Gtotal = totalAmount;
            ordersToUpdate.UpdateAt = DateTime.Now;
            context.Orders.Update(ordersToUpdate);
            context.SaveChanges();

            return RedirectToPage("/Customer/Restoran_Order_FandB/Order");
        }

        public IActionResult OnPost(IFormCollection form)
        {
            FoodStoreContext context = new FoodStoreContext();
            string orderItemId = form["orderItemId"];
            string orderId = form["orderId"];
            string accId = HttpContext.Session.GetString("accId");

            if (string.IsNullOrEmpty(accId))
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }

            if (int.TryParse(orderItemId, out int parsedOrderItemId) && int.TryParse(orderId, out int parsedOrderId))
            {
                var orderItem = context.OrderItems.FirstOrDefault(x => x.OrderItemId == parsedOrderItemId);
                var order = context.Orders.FirstOrDefault(x => x.OrderId == parsedOrderId);

                if (orderItem != null && order != null)
                {
                    var product = context.Products.FirstOrDefault(x => x.ProId == orderItem.ProductId);

                    if (product != null)
                    {
                        float Gtotal = (float)order.Gtotal - ((float)product.Price * (int)orderItem.Quantity);


                        context.OrderItems.Remove(orderItem);
                        context.SaveChanges();


                        listOrderItemsCheck = context.OrderItems
                            .Where(x => x.OrderId == parsedOrderId)
                            .ToList();

                        if (!listOrderItemsCheck.Any())
                        {

                            context.Orders.Remove(order);
                        }
                        else
                        {
                            order.Gtotal = Gtotal;
                            context.Orders.Update(order);
                        }

                        context.SaveChanges();
                    }
                    else
                    {
                        return Page();
                    }
                }
                else
                {
                    return Page();
                }
            }
            else
            {
                return Page();
            }
            string customerId = HttpContext.Session.GetString("accId");
            listOrder = context.Orders
                .Include(x => x.OrderItems)
                .Where(x => x.CustomerId == int.Parse(customerId) && x.Status == PendingStatus)
                .ToList();
            listOrderItems = context.OrderItems
                .Include(x => x.Product)
                .ToList();

            return Page();
        }

        //public async Task<IActionResult> OnGet()
        //{
        //    try
        //    {
        //        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        //        HttpResponseMessage orderResponse = await _httpClient.GetAsync("http://localhost:7031/api/Order/Customer/OrderStatus/1");
        //        if (orderResponse.IsSuccessStatusCode)
        //        {
        //            string orderData = await orderResponse.Content.ReadAsStringAsync();
        //            using JsonDocument doc = JsonDocument.Parse(orderData);
        //            if (doc.RootElement.TryGetProperty("$values", out JsonElement values))
        //            {
        //                listOrder = JsonSerializer.Deserialize<List<Order>>(values.GetRawText(), options) ?? new List<Order>();
        //            }
        //        }

        //        HttpResponseMessage itemResponse = await _httpClient.GetAsync("http://localhost:7031/api/Order/Customer/OrderItem");
        //        if (itemResponse.IsSuccessStatusCode)
        //        {
        //            string itemData = await itemResponse.Content.ReadAsStringAsync();
        //            using JsonDocument doc = JsonDocument.Parse(itemData);
        //            if (doc.RootElement.TryGetProperty("$values", out JsonElement values))
        //            {
        //                listOrderItems = JsonSerializer.Deserialize<List<OrderItem>>(values.GetRawText(), options) ?? new List<OrderItem>();
        //            }
        //        }
        //        return Page();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi khi lấy dữ liệu API: {Message}", ex.Message);
        //        return StatusCode(500, "Lỗi server: " + ex.Message);
        //    }
        //}
        //public async Task<IActionResult> OnPostCheckout(IFormCollection form)
        //{

        //    if (!float.TryParse(form["totalAmount"], out float totalAmount))
        //    {
        //        return Page();
        //    }

        //    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        //    HttpResponseMessage orderResponse = await _httpClient.GetAsync("http://localhost:7031/api/Order/Customer/Orders/1");
        //    var ordersToUpdate = new List<Order>();
        //    if (orderResponse.IsSuccessStatusCode)
        //    {
        //        string orderData = await orderResponse.Content.ReadAsStringAsync();
        //        using JsonDocument doc = JsonDocument.Parse(orderData);
        //        if (doc.RootElement.TryGetProperty("$values", out JsonElement values))
        //        {
        //            ordersToUpdate = JsonSerializer.Deserialize<List<Order>>(values.GetRawText(), options) ?? new List<Order>();
        //        }
        //    }

        //    if (!ordersToUpdate.Any())
        //    {
        //        return Page();
        //    }

        //    foreach (var order in ordersToUpdate)
        //    {
        //        order.Gtotal = totalAmount;
        //        int id = order.OrderId;
        //        var jsonContentUpdateOrder = JsonSerializer.Serialize(totalAmount); // Chỉ gửi totalAmount
        //        var httpContentUpdateOrder = new StringContent(jsonContentUpdateOrder, Encoding.UTF8, "application/json");

        //        string url = $"http://localhost:7031/api/Order/Customer/UpdateStatusGtotal/Order/{id}";

        //        HttpResponseMessage responseAddOrder = await _httpClient.PostAsync(url, httpContentUpdateOrder);
        //    }

        //    return RedirectToPage("/Customer/Restoran_Order_FandB/OrderConfirmation");
        //}
    }
}
