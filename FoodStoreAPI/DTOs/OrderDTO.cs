namespace FoodStoreAPI.DTO
{
    public class OrderDTO
    {
        public int? CustomerId { get; set; }
        public double? Gtotal { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? Status { get; set; }
        public DateTime? OrderDate { get; set; }
    }
}
