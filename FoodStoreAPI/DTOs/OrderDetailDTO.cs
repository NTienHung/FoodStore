using FoodStoreAPI.DTO;

namespace FoodStoreAPI.DTOs
{
    public class OrderDetailDTO
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public decimal Gtotal { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string Status { get; set; }
        public DateTime? OrderDate { get; set; }
        public List<OrderItemsDTO> OrderItems { get; set; }
    }
}
