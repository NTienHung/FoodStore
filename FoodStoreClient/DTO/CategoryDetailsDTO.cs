using FoodStoreAPI.Models;
using FoodStoreClient.DTO;
using System.Text.Json.Serialization;

namespace FoodStoreAPI.DTOs
{
    public class CategoryDetailsDTO
    {
        public int CateId { get; set; }
        public string? CateName { get; set; }
        public List<ProductByCategoryDTO> Products { get; set; }
    }
}
