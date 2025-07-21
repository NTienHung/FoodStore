using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FoodStoreAPI.Models;

namespace FoodStoreClient.Pages.Sale.Restoran_Management_Product
{
    public class Management_ProductModel : PageModel
    {
        public List<Product> listPro = new List<Product>();
        public List<Category> listCategory = new List<Category>();
        public decimal DiscountPercentage { get; set; } = 0.1m;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public string category { get; set; } = "";
        public string status { get; set; } = "";
        public string searchPro { get; set; } = "";
        public int minTo { get; set; } = 0;
        public int maxTo { get; set; } = 50000000;
        public async Task<IActionResult> OnGet(int? pageNumber, string? statusPage, string? categoryPage, string? search, string? from, string? to)
        {
            FoodStoreContext context = new FoodStoreContext();
            int totalProducts = 0;
            CurrentPage = pageNumber ?? 1;
            PageSize = 10;

            // Lấy toàn bộ sản phẩm, không filter accId
            var query = context.Products.Include(x => x.Cate).AsQueryable();

            // Nếu có filter giá
            if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
            {
                int min = int.Parse(from);
                int max = int.Parse(to);
                query = query.Where(x => x.Price >= min && x.Price <= max);
            }

            // Nếu có filter category
            if (!string.IsNullOrEmpty(categoryPage) && categoryPage != "-1")
            {
                int cateId = int.Parse(categoryPage);
                query = query.Where(x => x.Cate.CateId == cateId);
            }

            // Nếu có filter status
            if (!string.IsNullOrEmpty(statusPage) && statusPage != "-1")
            {
                query = query.Where(x => x.ProductStatus == statusPage);
            }

            // Nếu có filter search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower())
                    || x.ProId.ToString().Contains(search)
                    || x.Price.ToString().Contains(search)
                    || x.Quantity.ToString().Contains(search)
                    || x.ProductStatus.ToLower().Contains(search.ToLower())
                    || x.Unit.ToLower().Contains(search.ToLower()));
            }

            totalProducts = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);
            listPro = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            listCategory = context.Categories.ToList();
            category = categoryPage;
            status = statusPage;
            searchPro = search;
            minTo = from != null ? int.Parse(from) : 0;
            maxTo = to != null ? int.Parse(to) : 50000000;
            return Page();
        }
    }
}
