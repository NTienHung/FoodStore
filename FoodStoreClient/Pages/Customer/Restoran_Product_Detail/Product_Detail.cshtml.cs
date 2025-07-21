using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodStoreClient.Pages.Customer.Restoran_Product_Detail
{
    public class Product_DetailModel : PageModel
    {
        public int proId { get; set; }
        public IActionResult OnGet(string? id)
        {
            proId = int.Parse(id);
            return Page();
        }
    }
}
