using FoodStoreAPI.DTOs;
using System.Text.Json.Serialization;

namespace FoodStoreClient.DTO
{
    public class ProductWrapper
    {

        [JsonPropertyName("$values")]
        public List<ProductByCategoryDTO> Products { get; set; }
    }
}
