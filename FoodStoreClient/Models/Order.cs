using System.Text.Json.Serialization;

namespace FoodStoreClient.Models
{
    public partial class Order
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public double? Gtotal { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? Status { get; set; }
        public DateTime? OrderDate { get; set; }
    }
}
