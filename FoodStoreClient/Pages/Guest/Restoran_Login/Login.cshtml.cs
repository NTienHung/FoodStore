using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using FoodStoreClient.Models;
using Newtonsoft.Json;

namespace FoodStoreClient.Pages.Guest.Restoran_Login
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient client;
        private readonly string LoginApiUrl = "http://localhost:7031/api/Account/Guest/Account/Login";

        public Account account;

        public LoginModel()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IActionResult> OnPostAsync(IFormCollection form)
        {
            string username = form["username"];
            string password = form["password"];

            var requestUrl = $"{LoginApiUrl}?username={username}&pass={password}";
            var response = await client.GetAsync(requestUrl);
            var responseBody = await response.Content.ReadAsStringAsync();

            TempData["Username"] = username;
            TempData["Password"] = password;

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "API error: " + response.StatusCode;
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }

            if (string.IsNullOrWhiteSpace(responseBody))
            {
                TempData["ErrorMessage"] = "Empty response from API.";
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }

            var acc = JsonConvert.DeserializeObject<ResponseModel>(responseBody);

            if (acc == null)
            {
                TempData["ErrorMessage"] = "The username or password you entered is incorrect, please try again.";
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }

            HttpContext.Session.SetString("accId", acc.User.AccId.ToString());
            HttpContext.Session.SetString("roleId", acc.User.RoleId.ToString());
            HttpContext.Session.SetString("userName", acc.User.Username);
            HttpContext.Session.SetString("JWT", acc.Token);

            return acc.User.RoleId switch
            {
                1 => RedirectToPage("/Admin/Restoran_Management_Account/Management_Account"),
                2 => RedirectToPage("/Ship/Restoran_Management_Order/Management_Order"),
                3 => RedirectToPage("/Customer/Restoran_Home/Home"),
                4 => RedirectToPage("/Sale/Restoran_Management_Menu/Menu"),
                _ => RedirectToPage("/Guest/Restoran_Login/Login")
            };
        }
        public class ResponseModel
        {
            public string Token { get; set; }
            public Account User { get; set; }
        }
    }
}
