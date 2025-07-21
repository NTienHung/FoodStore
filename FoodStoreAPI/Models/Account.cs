using System;
using System.Collections.Generic;

namespace FoodStoreAPI.Models
{
    public partial class Account
    {
        public Account()
        {
            MessagessFromUsers = new HashSet<Messagess>();
            MessagessToUsers = new HashSet<Messagess>();
            Orders = new HashSet<Order>();
            ProductReviews = new HashSet<ProductReview>();
            Products = new HashSet<Product>();
            Revenues = new HashSet<Revenue>();
            Tokens = new HashSet<Token>();
            Transactions = new HashSet<Transaction>();
        }

        public int AccId { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public double? Wallet { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? RoleId { get; set; }
        public string? Address { get; set; }

        public virtual Role? Role { get; set; }
        public virtual AccountShipped? AccountShipped { get; set; }
        public virtual ICollection<Messagess> MessagessFromUsers { get; set; }
        public virtual ICollection<Messagess> MessagessToUsers { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Revenue> Revenues { get; set; }
        public virtual ICollection<Token> Tokens { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
