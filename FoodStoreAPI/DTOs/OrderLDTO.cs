namespace FoodStoreAPI.DTO
{
    public class OrderLDTO
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public double? Gtotal { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? Status { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerAddress { get; set; }
    }
}
