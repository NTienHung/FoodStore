using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoodStoreClient.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using FoodStoreAPI.DTO;

namespace FoodStoreClient.Pages.Admin.Restoran_Management_Order
{
    public class Management_OrderModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string GetOrderApiUrl = "http://localhost:7031/api/Order/Admin/GetAll";

        public string errorMessage { get; set; } = "";
        public DateTime? dateNow { get; set; } = DateTime.Now;
        public DateTime? datePast { get; set; } = DateTime.Now.AddYears(-2);
        public DateTime? dateSevenDay { get; set; } = new DateTime(2024, 4, 1);

        public List<OrderLDTO> Orders { get; set; } = new List<OrderLDTO>();

        public List<OrderLDTO> FilteredOrders { get; set; } = new List<OrderLDTO>();

        public Management_OrderModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IActionResult> OnGet()
        {
            string accId = HttpContext.Session.GetString("accId");
            if (accId == null)
                return RedirectToPage("/Guest/Restoran_Login/Login");

            await LoadAllOrders();
            FilteredOrders = FilterOrders(dateSevenDay, dateNow);
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

            await LoadAllOrders();
            FilteredOrders = FilterOrders(fromDate, toDate);
            dateSevenDay = fromDate;
            dateNow = toDate;

            return Page();
        }

        private async Task LoadAllOrders()
        {
            try
            {
                var response = await client.GetAsync(GetOrderApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(responseBody);
                    var valuesJson = jsonDoc.RootElement.GetProperty("$values").ToString();
                    Orders = JsonSerializer.Deserialize<List<OrderLDTO>>(valuesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrderLDTO>();
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



        public List<OrderLDTO> FilterOrders(DateTime? fromDate, DateTime? toDate)
        {
            return Orders
                .Where(x => x.OrderDate >= fromDate && x.OrderDate <= toDate)
                .Select(x => new OrderLDTO
                {
                    OrderId = x.OrderId,
                    CustomerId = x.CustomerId,
                    CustomerName = x.CustomerName,
                    Gtotal = x.Gtotal,
                    Status = x.Status,
                    OrderDate = x.OrderDate,
                    CreateAt = x.CreateAt
                }).ToList();
        }





    }
}
