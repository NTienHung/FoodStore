using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodStoreClient.Pages.Sale.Restoran_Logout
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("accId") != null)
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
            return Page();
        }
    }
}
