namespace POS1.Data
{
    public class ProductType
    {
        public int Id { get; set; }
        public string TypeName { get; set; }

        // Foreign key
        public int TenantId { get; set; }

        // Navigation property
        public Tenant Tenant { get; set; }
    }
}
