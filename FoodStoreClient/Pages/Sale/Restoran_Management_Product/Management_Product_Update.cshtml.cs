using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoodStoreClient.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FoodStoreClient.Pages.Sale.Restoran_Management_Product
{
    public class Management_Product_UpdateModel : PageModel
    {
        public Product Product { get; set; }
        private readonly HttpClient client;
        private readonly string ProductApiUrl = "http://localhost:7031/api/Product/Sale/Product";
        private readonly string UpdateProductApiUrl = "http://localhost:7031/api/Product/Sale/UpdateProduct";
        public Management_Product_UpdateModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
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
        public async Task<IActionResult> OnPost(IFormCollection form)
        {
            string proId = form["proId"];
            string name = form["name"];
            string price = form["price"];
            string status = form["status"];
            string quantity = form["quantity"];
            string unit = form["unit"];
            string categpry = form["categpry"];
            string image = form["image"];
            string accId = HttpContext.Session.GetString("accId");
            bool isPriceValid = float.TryParse(price, out float parsedPrice);
            bool isQuantityValid = int.TryParse(quantity, out int parsedQuantity);
            if (accId != null)
            {
                var requestUrl = $"{ProductApiUrl}?proId={int.Parse(proId)}";
                var response = await client.GetAsync(requestUrl);
                var responseBody = await response.Content.ReadAsStringAsync();
                Product = JsonSerializer.Deserialize<Product>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (Product != null)
                {
                    if (isPriceValid && isQuantityValid)
                    {
                        if (parsedPrice < 100000000 && parsedPrice > 0 &&
                        parsedQuantity > 0 && parsedQuantity < 10000)
                        {
                            Product.Name = name;
                            Product.Price = Decimal.Parse(price.ToString());
                            Product.ProductStatus = status;
                            Product.Quantity = int.Parse(quantity);
                            Product.Unit = unit;
                            Product.CateId = int.Parse(categpry);
                            Product.Images = image;
                            Product.UpdateAt = DateTime.Now;
                            
                            var content = new StringContent(JsonSerializer.Serialize(Product), Encoding.UTF8, "application/json");
                            string requestlUrl = $"{UpdateProductApiUrl}/{proId}";
                            var responseUpdate = await client.PutAsync(requestlUrl, content);
                            if (!responseUpdate.IsSuccessStatusCode)
                            {
                                TempData["ErrorMessage"] = $"API Register fail: {responseUpdate.StatusCode}";
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Data added successfully";
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Product price or product quantity exceeds the allowed level!";
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Price and Quantity must be numbers!";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Product does not exist!";
                }
            }
            else
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
            var requestUrl1 = $"{ProductApiUrl}?proId={int.Parse(proId)}";
            var response1 = await client.GetAsync(requestUrl1);
            var responseBody1 = await response1.Content.ReadAsStringAsync();
            Product = JsonSerializer.Deserialize<Product>(responseBody1, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Redirect("/Sale/Restoran_Management_Product/Management_Product_Update?id=" + proId);
        }
    }
}
