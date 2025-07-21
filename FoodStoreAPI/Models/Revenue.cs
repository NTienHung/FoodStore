using System;
using System.Collections.Generic;

namespace FoodStoreAPI.Models
{
    public partial class Revenue
    {
        public int RevenueId { get; set; }
        public double? RevenuePrice { get; set; }
        public double? InterestRate { get; set; }
        public double? FloorFee { get; set; }
        public DateTime? CreateAt { get; set; }
        public int? AccId { get; set; }

        public virtual Account? Acc { get; set; }
    }
}
