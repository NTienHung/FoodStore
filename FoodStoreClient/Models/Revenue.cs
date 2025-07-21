namespace FoodStoreClient.Models
{
    public class Revenue
    {
        public int RevenueId { get; set; }
        public double? RevenuePrice { get; set; }
        public double? InterestRate { get; set; }
        public double? FloorFee { get; set; }
        public DateTime? CreateAt { get; set; }
        public int? AccId { get; set; }
    }
}
