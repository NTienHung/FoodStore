using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace FoodStoreClient.Pages.Sale.Restoran_Management_Product
{
    public class Management_Product_DeleteModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string UpdateApiUrl = "http://localhost:7031/api/Product/Sale/DeleteProduct";
        public Management_Product_DeleteModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<IActionResult> OnGet(string proId)
        {

            string accId = HttpContext.Session.GetString("accId");
            if (accId != null)
            {
                var requestCheckUrl = $"{UpdateApiUrl}?proId={int.Parse(proId)}";
                var responseCheck = await client.PutAsync(requestCheckUrl, null);
                return RedirectToPage("/Sale/Restoran_Management_Product/Management_Product");
            }
            else
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
        }
    }
}
