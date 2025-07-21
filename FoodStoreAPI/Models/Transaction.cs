using System;
using System.Collections.Generic;

namespace FoodStoreAPI.Models
{
    public partial class Transaction
    {
        public Transaction()
        {
            AccountShippeds = new HashSet<AccountShipped>();
        }

        public int TransId { get; set; }
        public int? AccId { get; set; }
        public int? ProId { get; set; }
        public DateTime? TransDate { get; set; }
        public decimal? Amount { get; set; }
        public string? Status { get; set; }

        public virtual Account? Acc { get; set; }
        public virtual Product? Pro { get; set; }
        public virtual ICollection<AccountShipped> AccountShippeds { get; set; }
    }
}
