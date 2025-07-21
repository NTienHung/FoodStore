namespace FoodStoreAPI.DTO
{
    public class productLDTO
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Unit { get; set; }
        public string? Images { get; set; }
        public int? Quantity { get; set; }
        public int? CateId { get; set; }
        public string? ProductStatus { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
