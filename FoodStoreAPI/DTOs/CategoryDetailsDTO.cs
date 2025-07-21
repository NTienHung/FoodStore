using FoodStoreAPI.Models;

namespace FoodStoreAPI.DTOs
{
    public class CategoryDetailsDTO
    {
        public int CateId { get; set; }
        public string? CateName { get; set; }

        public ICollection<ProductByCategoryDTO> Products { get; set; }
    }
}
