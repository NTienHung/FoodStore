using System;
using System.Collections.Generic;

namespace FoodStoreAPI.Models
{
    public partial class Messagess
    {
        public int MessId { get; set; }
        public int? FromUserId { get; set; }
        public int? ToUserId { get; set; }
        public string? MessageText { get; set; }
        public DateTime? SentTime { get; set; }

        public virtual Account? FromUser { get; set; }
        public virtual Account? ToUser { get; set; }
    }
}
