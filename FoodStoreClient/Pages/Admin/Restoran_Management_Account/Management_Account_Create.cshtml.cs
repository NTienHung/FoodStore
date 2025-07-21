using FoodStoreAPI.Models;
using FoodStoreClient.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodStoreClient.Pages.Admin.Restoran_Management_Account
{
    public class Management_Account_CreateModel : PageModel
    {
        private readonly HttpClient client;
        private readonly FoodStoreContext foodStoreContext = new FoodStoreContext();
        public List<RoleDTO> listRole = new List<RoleDTO>();
        [BindProperty]
        public AccountDTO account { get; set; } = new AccountDTO();
        public async Task<ActionResult> OnGet()
        {
            listRole = foodStoreContext.Roles.Where(x => x.RoleId != 1).Select(x => new RoleDTO
            {
                RoleId = x.RoleId,
                RoleName = x.RoleName
            }).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var httpClient = new HttpClient();
            var apiUrl = "http://localhost:7031/api/Account/Admin/AddNewUser/Account";

            var response = await httpClient.PostAsJsonAsync(apiUrl, account);

            if (response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Thêm tài khoản thành công!";
                return Page();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = errorContent;
                return Page();
            }
        }
    }
}
