using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FoodStoreClient.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodStoreClient.Pages.Sale.Restoran_Revenue_Report
{
    public class ReportModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string RevenueCaculateApiUrl = "http://localhost:7031/api/Order/Sale/RevenueCaculate";
        private readonly string OrderCloseCaculateApiUrl = "http://localhost:7031/api/Order/Sale/OrderCloseCaculate";
        public string errorMessage { get; set; } = "";
        public DateTime? dateNow { get; set; } = DateTime.Now.AddDays(+1);
        public DateTime? datePast { get; set; } = DateTime.Now.AddYears(-2);
        public DateTime? dateSevenDay { get; set; } = DateTime.Now.AddDays(-7);
        public List<OrderItem> product { get; set; } = new List<OrderItem>();
        public List<Revenue> revenue { get; set; } = new List<Revenue>();
        public float revenuePrice { get; set; } = 0;
        public float interestRate { get; set; } = 0;
        public float FloorFee { get; set; } = 0;

        private int accId = 4; 

        public ReportModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IActionResult> OnGet()
        {
            string requestOrderItemsUrl = $"{OrderCloseCaculateApiUrl}?accId={accId}&dateNow={dateNow}&dateSevenDay={dateSevenDay}";
            var responseOrderItems = await client.GetAsync(requestOrderItemsUrl);
            if (!responseOrderItems.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = $"API Error: {responseOrderItems.StatusCode}";
                return Page();
            }
            var responseOrderItemsBody = await responseOrderItems.Content.ReadAsStringAsync();
            product = DeserializeOrderItems(responseOrderItemsBody);

            string requestRevenueUrl = $"{RevenueCaculateApiUrl}?accId={accId}&dateNow={dateNow}&dateSevenDay={dateSevenDay}";
            var responseRevenue = await client.GetAsync(requestRevenueUrl);
            if (!responseRevenue.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = $"API Error: {responseRevenue.StatusCode}";
                return Page();
            }
            var responseRevenueBody = await responseRevenue.Content.ReadAsStringAsync();
            revenue = DeserializeRevenue(responseRevenueBody);

            CalculateTotals();
            return Page();
        }

        public async Task<IActionResult> OnPost(string? from, string? to)
        {
            if (!DateTime.TryParse(from, out DateTime fromDate) || !DateTime.TryParse(to, out DateTime toDate))
            {
                return Page();
            }

            string requestOrderItemsUrl = $"{OrderCloseCaculateApiUrl}?accId={accId}&dateNow={to}&dateSevenDay={from}";
            var responseOrderItems = await client.GetAsync(requestOrderItemsUrl);
            if (!responseOrderItems.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = $"API Error: {responseOrderItems.StatusCode}";
                return Page();
            }
            var responseOrderItemsBody = await responseOrderItems.Content.ReadAsStringAsync();
            product = DeserializeOrderItems(responseOrderItemsBody);

            string requestRevenueUrl = $"{RevenueCaculateApiUrl}?accId={accId}&dateNow={to}&dateSevenDay={from}";
            var responseRevenue = await client.GetAsync(requestRevenueUrl);
            if (!responseRevenue.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = $"API Error: {responseRevenue.StatusCode}";
                return Page();
            }
            var responseRevenueBody = await responseRevenue.Content.ReadAsStringAsync();
            revenue = DeserializeRevenue(responseRevenueBody);

            CalculateTotals();
            dateSevenDay = fromDate;
            dateNow = toDate;

            return Page();
        }

        private List<OrderItem> DeserializeOrderItems(string json)
        {
            if (json.Trim().StartsWith("["))
            {
                return JsonSerializer.Deserialize<List<OrderItem>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrderItem>();
            }
            else
            {
                var jsonDoc = JsonDocument.Parse(json);
                var valuesJson = jsonDoc.RootElement.GetProperty("$values").ToString();
                return JsonSerializer.Deserialize<List<OrderItem>>(valuesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrderItem>();
            }
        }

        private List<Revenue> DeserializeRevenue(string json)
        {
            if (json.Trim().StartsWith("["))
            {
                return JsonSerializer.Deserialize<List<Revenue>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Revenue>();
            }
            else
            {
                var jsonDoc = JsonDocument.Parse(json);
                var valuesJson = jsonDoc.RootElement.GetProperty("$values").ToString();
                return JsonSerializer.Deserialize<List<Revenue>>(valuesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Revenue>();
            }
        }

        private void CalculateTotals()
        {
            revenuePrice = 0;
            interestRate = 0;
            FloorFee = 0;

            if (revenue.Count > 0)
            {
                foreach (var item in revenue)
                {
                    revenuePrice += (float)item.RevenuePrice;
                    interestRate += (float)item.InterestRate;
                    FloorFee += (float)item.FloorFee;
                }
            }
        }
    }
}