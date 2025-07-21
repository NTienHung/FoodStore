using System;
using System.Collections.Generic;

namespace FoodStoreAPI.Models
{
    public partial class Token
    {
        public int TokenId { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? TokenString { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? Status { get; set; }
        public int? AccId { get; set; }

        public virtual Account? Acc { get; set; }
    }
}
