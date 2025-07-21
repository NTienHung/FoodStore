using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FoodStoreAPI.Models;

namespace FoodStoreClient.Pages.Sale.Restoran_Management_Order
{
    public class Management_Order_ViewModel : PageModel
    {
        private readonly FoodStoreContext _context;

        public Management_Order_ViewModel(FoodStoreContext context)
        {
            _context = context;
        }

        public Order Order { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public int TotalItem { get; set; }
        public float GrandTotal { get; set; }
        public float CapitalTotal { get; set; }

        public async Task<IActionResult> OnGetAsync(string? orderId)
        {
            if (!IsLoggedIn()) return RedirectToPage("/Guest/Restoran_Login/Login");

            if (!await LoadOrderData(orderId)) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? orderId, string? statusOrder)
        {
            if (!IsLoggedIn()) return RedirectToPage("/Guest/Restoran_Login/Login");

            if (!await LoadOrderData(orderId)) return NotFound();

            if (Order != null)
            {
                Order.Status = statusOrder;
                Order.UpdateAt = DateTime.Now;
                _context.Orders.Update(Order);
                await _context.SaveChangesAsync();
            }

            if (statusOrder == "Close Order")
            {
                await UpdateRevenue();
            }

            return Page();
        }

        private async Task<bool> LoadOrderData(string? orderId)
        {
            if (orderId == null) return false;

            int id = int.Parse(orderId);

            Order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (Order == null) return false;

            OrderItems = await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.OrderId == id)
                .ToListAsync();

            TotalItem = (int)OrderItems.Sum(oi => oi.Quantity);

            return true;
        }

        private async Task UpdateRevenue()
        {
            string accIdStr = "4";
            Console.WriteLine(accIdStr);
            if (string.IsNullOrEmpty(accIdStr)) return;
            int accId = int.Parse(accIdStr);

            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            var todayProducts = await _context.OrderItems
                .Include(x => x.Order)
                .Include(x => x.Product)
                .Where(x => x.Order.UpdateAt >= today && x.Order.UpdateAt < tomorrow
                            && x.Order.Status == "Close Order"
                            && x.Product.AccId == accId)
                .ToListAsync();

            GrandTotal = (float)todayProducts.Sum(p => p.Product.Price * p.Quantity);
            CapitalTotal = (float)todayProducts.Sum(p => p.Product.OriginalPrice * p.Quantity);

            var revenue = await _context.Revenues
                .FirstOrDefaultAsync(r => r.AccId == accId && r.CreateAt >= today && r.CreateAt < tomorrow);

            if (revenue != null)
            {
                revenue.RevenuePrice = GrandTotal;
                revenue.InterestRate = GrandTotal - CapitalTotal;
                revenue.FloorFee = (GrandTotal - CapitalTotal) * 0.05f;
                _context.Revenues.Update(revenue);
            }
            else
            {
                var newRevenue = new Revenue
                {
                    AccId = accId,
                    CreateAt = DateTime.Now,
                    RevenuePrice = GrandTotal,
                    InterestRate = GrandTotal - CapitalTotal,
                    FloorFee = (GrandTotal - CapitalTotal) * 0.05f
                };
                _context.Revenues.Add(newRevenue);
            }

            await _context.SaveChangesAsync();
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("accId"));
        }
    }
}
