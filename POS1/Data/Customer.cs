namespace POS1.Data
{
    public class Customer
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Cnic { get; set; }
        public Tenant Tenant { get; set; }


    }
}
