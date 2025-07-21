using System;
using System.Collections.Generic;

namespace FoodStoreAPI.Models
{
    public partial class ProductReview
    {
        public int ReviewId { get; set; }
        public string? Images { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? Star { get; set; }
        public int? ProId { get; set; }
        public int? AccId { get; set; }

        public virtual Account? Acc { get; set; }
        public virtual Product? Pro { get; set; }
    }
}
