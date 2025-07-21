using FoodStoreAPI.DTO;
using FoodStoreAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FoodStoreClient.Pages.Admin.Restoran_Management_Category
{
    public class Management_Category_UpdateModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string cateApiUrl = "http://localhost:7031/api/Categorys/Admin/Categorys";
        public CategoryDetailsDTO cateInfo = new CategoryDetailsDTO();
        public List<ProductByCategoryDTO> proList = new List<ProductByCategoryDTO>();
        public int categoryId { get; set; } = 0;
        public Management_Category_UpdateModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IActionResult> OnGet(string cateId)
        {
            string accId = HttpContext.Session.GetString("accId");
            if (accId == null)
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
            if (string.IsNullOrEmpty(cateId))
            {
                TempData["ErrorMessage"] = "Category ID is required.";
                return Page();
            }
            else
            {
                categoryId = int.Parse(cateId);
            }
            try
            {
                var requestUrl = $"{cateApiUrl}/{categoryId}";
                var response = await client.GetAsync(requestUrl);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"API Error: {response.StatusCode}";
                    return Page();
                }
                var responseBody = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(responseBody);

                var root = doc.RootElement;
                var cateID = root.GetProperty("cateId").GetInt32();
                var cateName = root.GetProperty("cateName").GetString();
                var products = root.GetProperty("products").GetProperty("$values");
                var productList = new List<ProductByCategoryDTO>();

                foreach (var product in products.EnumerateArray())
                {
                    var productDto = new ProductByCategoryDTO();
                    productDto.ProId = product.GetProperty("proId").GetInt32();
                    productDto.Name = product.TryGetProperty("name", out var name) ? name.GetString() : null;
                    productDto.Price = product.TryGetProperty("price", out var price) ? price.GetDecimal() : 0m;
                    productDto.OriginalPrice = product.TryGetProperty("originalPrice", out var originalPrice) ? originalPrice.GetDouble() : 0d;
                    productDto.Unit = product.TryGetProperty("unit", out var unit) ? unit.GetString() : null;
                    productDto.Images = product.TryGetProperty("images", out var images) ? images.GetString() : null;
                    productDto.Quantity = product.TryGetProperty("quantity", out var quantity) ? quantity.GetInt32() : 0;
                    productDto.ProductStatus = product.TryGetProperty("productStatus", out var productStatus) ? productStatus.GetString() : null;
                    productDto.DiscountStartTime = product.TryGetProperty("discountStartTime", out var discountStart) && discountStart.ValueKind != JsonValueKind.Null ? discountStart.GetDateTime() : (DateTime?)null;
                    productDto.DiscountEndTime = product.TryGetProperty("discountEndTime", out var discountEnd) && discountEnd.ValueKind != JsonValueKind.Null ? discountEnd.GetDateTime() : (DateTime?)null;
                    productDto.DiscountPercentage = product.TryGetProperty("discountPercentage", out var discountPercentage) && discountPercentage.ValueKind != JsonValueKind.Null ? discountPercentage.GetDecimal() : (decimal?)0;
                    productDto.CreateAt = product.TryGetProperty("createAt", out var createAt) && createAt.ValueKind != JsonValueKind.Null ? createAt.GetDateTime() : (DateTime?)null;
                    productDto.UpdateAt = product.TryGetProperty("updateAt", out var updateAt) && updateAt.ValueKind != JsonValueKind.Null ? updateAt.GetDateTime() : (DateTime?)null;

                    productList.Add(productDto);
                }
                cateInfo = new CategoryDetailsDTO();
                cateInfo.CateId = cateID;
                cateInfo.CateName = cateName;
                cateInfo.Products = productList;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching profile data.";
                return Page();
            }

            // Ensure all code paths return a value
            return Page();
        }
        public async Task<IActionResult> OnPost(IFormCollection form)
        {
            var cateId = form["cateId"];
            var cateName = form["name"];
            int cateIdValue = int.Parse(cateId);
            if (string.IsNullOrEmpty(cateName))
            {
                TempData["ErrorMessage"] = "Category ID and Name are required.";
                return Page();
            }
            var categoryDTO = new CategoryDTO
            {
                CateName = cateName
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string urlUpdate = "http://localhost:7031/api/Categorys/Admin/UpdateCategory";
                var requestUrl = $"{urlUpdate}/{cateIdValue}";
                string jsonData = JsonSerializer.Serialize(categoryDTO);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(requestUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Update failed!";
                    return RedirectToPage("/Admin/Restoran_Management_Category/Management_Category_Update?cateId=" + cateId);
                }
                TempData["ErrorMessage"] = "Update successfully!";
                return RedirectToPage("/Admin/Restoran_Management_Category/Management_Category_Update?cateId=" + cateId);
            }
        }
    }
}
