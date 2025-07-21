using System;
using System.Collections.Generic;

namespace FoodStoreAPI.Models
{
    public partial class AccountShipped
    {
        public int AccId { get; set; }
        public int? TransId { get; set; }
        public string? AddressShop { get; set; }
        public string? AddressCustomer { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? Status { get; set; }

        public virtual Account Acc { get; set; } = null!;
        public virtual Transaction? Trans { get; set; }
    }
}
