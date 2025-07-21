namespace FoodStoreAPI.DTO
{
    public class AccountDTO
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public DateTime? CreateAt { get; set; }
        public int? RoleId { get; set; }
        public string? Address { get; set; }

    }
}
