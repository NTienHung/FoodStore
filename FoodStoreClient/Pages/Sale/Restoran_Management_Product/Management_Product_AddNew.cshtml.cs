using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoodStoreClient.Models;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FoodStoreClient.Pages.Sale.Restoran_Management_Product
{
    public class Management_Product_AddNewModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string CategoryApiUrl = "http://localhost:7031/api/Product/Sale/Categories";
        private readonly string AddNewProductApiUrl = "http://localhost:7031/api/Product/Sale/AddNewProduct";
        private readonly string ProductApiUrl = "http://localhost:7031/api/Product/Sale/ProductByLastList";
        public string errorMessage { get; set; } = "";
        public string Name { get; set; } = "";
        public string Price { get; set; } = "";
        public string Image { get; set; } = "";
        public string Unit { get; set; } = "";
        public string Quantity { get; set; } = "";
        public string Status { get; set; } = "";
        public string Category { get; set; } = "";
        public Product newPro { get; set; } = new Product();
        public List<Category> listCate = new List<Category>();
        public Management_Product_AddNewModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<IActionResult> OnGet(string proId)
        {
            string accId = HttpContext.Session.GetString("accId");
            if (accId != null)
            {
                string requestCatelUrl = $"{CategoryApiUrl}";
                var responseCate = await client.GetAsync(requestCatelUrl);
                if (!responseCate.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"API Error: {responseCate.StatusCode}";
                    return Page();
                }

                var responseCateBody = await responseCate.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(responseCateBody);

         
                if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
                {
                   
                    listCate = JsonSerializer.Deserialize<List<Category>>(jsonDoc.RootElement.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Category>();
                }
                else
                {
                    TempData["ErrorMessage"] = "$values không phải là mảng!";
                }

                return Page();
            }
            else
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
        }


        public async Task<IActionResult> OnPost(string? name, string? price, string? unit, string? quantity, string? status, string? category, string? image, string? priceOriginal)
        {
            // Bỏ kiểm tra accId
            Name = name;
            Price = price;
            Unit = unit;
            Quantity = quantity;
            Status = status;
            Category = category;
            Image = image;

            bool isPriceValid = decimal.TryParse(Price, out decimal parsedPrice);
            bool isQuantityValid = int.TryParse(Quantity, out int parsedQuantity);
            var Product = new Product();
            if (isPriceValid && isQuantityValid)
            {
                if (parsedPrice < 100000000 && parsedPrice > 0 &&
                    parsedQuantity > 0 && parsedQuantity < 10000)
                {
                    Product.Name = Name;
                    Product.Price = parsedPrice;
                    Product.ProductStatus = Status;
                    Product.Quantity = parsedQuantity;
                    Product.Unit = unit;
                    Product.CateId = int.Parse(Category);
                    Product.CreateAt = DateTime.Now;
                    Product.UpdateAt = DateTime.Now;
                    Product.Images = Image;
                    // Không gán Product.AccId
                    var content = new StringContent(JsonSerializer.Serialize(Product), Encoding.UTF8, "application/json");
                    var responseAddNew = await client.PostAsync(AddNewProductApiUrl, content);
                    if (!responseAddNew.IsSuccessStatusCode)
                    {
                        var errorContent = await responseAddNew.Content.ReadAsStringAsync();
                        TempData["ErrorMessage"] = $"API Register fail: {responseAddNew.StatusCode} - {errorContent}";
                        return Page();
                    }
                    var responseBody = await responseAddNew.Content.ReadAsStringAsync();
                    errorMessage = "Data added successfully";
                }
                else
                {
                    errorMessage = "Product price, product quantity, or product original price exceeds the allowed limits!";
                }
            }
            else
            {
                errorMessage = "Price and Quantity must be valid numbers!";
            }

            return Page();
        }
    }
    }
