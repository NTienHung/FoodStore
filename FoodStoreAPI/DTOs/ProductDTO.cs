namespace FoodStoreAPI.DTO
{
    public class ProductDTO
    {
        public int ProId { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public double? OriginalPrice { get; set; }
        public string? Unit { get; set; }
        public string? Images { get; set; }
        public int? Quantity { get; set; }
        public string? ProductStatus { get; set; }
        public int? AccId { get; set; }
        public int? CateId { get; set; }
        public string? CateName { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? NameAccount { get; set; }
        public DateTime? DiscountStartTime { get; set; }
        public DateTime? DiscountEndTime { get; set; }
        public decimal? DiscountPercentage { get; set; }
    }
}
