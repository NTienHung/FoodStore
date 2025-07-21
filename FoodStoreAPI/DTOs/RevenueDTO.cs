namespace FoodStoreAPI.DTO
{
    public class RevenueDTO
    {
        public int RevenueId { get; set; }
        public double? RevenuePrice { get; set; }
        public double? InterestRate { get; set; }
        public double? FloorFee { get; set; }
        public DateTime? CreateAt { get; set; }
        public int? AccId { get; set; }
    }
}
