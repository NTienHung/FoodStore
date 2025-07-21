using System;
using System.Collections.Generic;

namespace FoodStoreAPI.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public double? Gtotal { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? Status { get; set; }
        public DateTime? OrderDate { get; set; }

        public virtual Account? Customer { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
