namespace FoodStoreAPI.DTO
{
    public class AccountLDTO
    {
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
        public virtual RoleDTO? Role { get; set; }
        public string RoleName { get; set; }
    }
}
