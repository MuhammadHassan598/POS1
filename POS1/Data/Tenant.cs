namespace POS1.Data
{
    public class Tenant
    {
        public int TenantID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactInformation { get; set; }



        // Navigation properties
        public ICollection<User> Users { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Sale> Sales { get; set; }
        public ICollection<SaleItem> SaleItems { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<InventoryLog> InventoryLogs { get; set; }

    }
}
