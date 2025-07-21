using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoodStoreClient.DTO;
using FoodStoreClient.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FoodStoreClient.Pages.Customer.Restoran_Menu
{
    public class MenuModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string AddNewOrderApiUrl = "http://localhost:7031/api/Order/Customer/Add/Order";
        private readonly string OrderApiUrl = "http://localhost:7031/api/Order/Customer/View/OrderByGtotalCustomerStatus";
        private readonly string AddNewOrderItemsApiUrl = "http://localhost:7031/api/Order/Customer/Add/OrderItem";
        private readonly string ListProApiUrl = "http://localhost:7031/api/Product/Customer/Product/On";
        private readonly string ListCateApiUrl = "http://localhost:7031/api/Product/Customer/Product/Category";
        public List<Product> pro = new List<Product>();
        public List<Category> cate = new List<Category>();
        public MenuModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<IActionResult> OnGet()
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Common/403");
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string accId = HttpContext.Session.GetString("accId");
            Console.WriteLine(accId);
            if (accId != null)
            {
                var requestUrl = $"{ListCateApiUrl}";
                var response = await client.GetAsync(requestUrl);
                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(responseBody);
                var valuesJson = jsonDoc.RootElement.GetProperty("$values").ToString();
                cate = JsonSerializer.Deserialize<List<Category>>(valuesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Category>();

                var requestUrl1 = $"{ListProApiUrl}";
                var response1 = await client.GetAsync(requestUrl1);
                var responseBody1 = await response1.Content.ReadAsStringAsync();
                var jsonDoc1 = JsonDocument.Parse(responseBody1);
                var valuesJson1 = jsonDoc1.RootElement.GetProperty("$values").ToString();
                pro = JsonSerializer.Deserialize<List<Product>>(valuesJson1, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Product>();
            }
            else
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
            return Page();
        }
        [BindProperty]
        public Dictionary<int, SelectedProduct> SelectedProducts { get; set; }
        public async Task<IActionResult> OnPost()
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Common/403");
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string accId = HttpContext.Session.GetString("accId");
            Console.WriteLine("accId:"+accId);
            if (accId == null)
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
            else
            {
                if (SelectedProducts != null)
                {
                    int accid = int.Parse(accId);
                    float gtotalOrder = 0;
                    var insertOrder = new Order
                    {
                        CustomerId = accid,
                        Gtotal = gtotalOrder,
                        CreateAt = DateTime.Now,
                        Status = "Pending",
                        OrderDate = DateTime.Now,
                    };
                    var content = new StringContent(JsonSerializer.Serialize(insertOrder), Encoding.UTF8, "application/json");
                    var responseAddNew = await client.PostAsync(AddNewOrderApiUrl, content);
                    if (!responseAddNew.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = $"API Register fail: {responseAddNew.StatusCode}";
                        return Page();
                    }
                    var responseBody = await responseAddNew.Content.ReadAsStringAsync();

                    var requestUrl = $"{OrderApiUrl}?gtotalOrder={gtotalOrder}&accid={int.Parse(accId)}";
                    var response = await client.GetAsync(requestUrl);
                    var responseBody1 = await response.Content.ReadAsStringAsync();
                    var newOrder = JsonSerializer.Deserialize<OrderDTO>(responseBody1, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    foreach (var proOrder in SelectedProducts)
                    {
                        if (proOrder.Value.ProductId != null && proOrder.Value.Quantity != null)
                        {
                            if (proOrder.Value.ProductId != 0)
                            {
                                var insertOrderItems = new OrderItem
                                {
                                    OrderId = newOrder.OrderId,
                                    ProductId = proOrder.Value.ProductId,
                                    Quantity = proOrder.Value.Quantity,
                                    CreateAt = DateTime.Now,
                                    UpdateAt = DateTime.Now,
                                };
                                var content1 = new StringContent(JsonSerializer.Serialize(insertOrderItems), Encoding.UTF8, "application/json");
                                var responseAddNew1 = await client.PostAsync(AddNewOrderItemsApiUrl, content1);
                                Console.WriteLine(responseAddNew1);
                                if (!responseAddNew1.IsSuccessStatusCode)
                                {
                                    TempData["ErrorMessage"] = $"API Register fail: {responseAddNew1.StatusCode}";
                                    return Page();
                                }
                                var responseBody3 = await responseAddNew1.Content.ReadAsStringAsync();
                            }
                        }
                    }
                }
            }
            return RedirectToPage("/Customer/Restoran_Order_FandB/Cart");
        }
    }
}
