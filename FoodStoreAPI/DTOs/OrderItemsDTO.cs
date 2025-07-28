namespace FoodStoreAPI.DTOs
{
    public class OrderItemsDTO
    {
        public int? OrderItemId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Images { get; set; }
        public double? OriginalPrice { get; set; }
        public string? CategoryName { get; set; }
    }
}
