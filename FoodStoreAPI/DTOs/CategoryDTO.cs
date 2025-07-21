using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FoodStoreAPI.DTO
{
    public class CategoryDTO
    {
        public int CateId { get; set; }

        public string? CateName { get; set; }
    }
}
