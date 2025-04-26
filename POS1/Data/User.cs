namespace POS1.Data
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public string Email { get; set; }
        public UserRole RoleIs { get; set; }

        // Foreign key
        public int TenantID { get; set; }

        // Navigation properties
        public Tenant Tenant { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }
    public enum UserRole
    {
        Admin,
        Cashier
 
    }
}
