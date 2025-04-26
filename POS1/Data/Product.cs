namespace POS1.Data
{
    public class Product
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public QuantityUnit QuantityUnit { get; set; }
        public string? Barcode { get; set; }
        public bool IsActive { get; set; }

        // Foreign keys
        public int TenantId { get; set; }
        public int ProductTypeId { get; set; }  

        // Navigation properties
        public Tenant Tenant { get; set; }
        public ProductType ProductType { get; set; } // New navigation property
        public ICollection<Stock> Stocks { get; set; }

        public ICollection<SaleItem> SaleItems { get; set; }
        public ICollection<InventoryLog> InventoryLogs { get; set; }
    }
   

}
