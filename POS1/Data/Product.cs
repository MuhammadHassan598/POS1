namespace POS1.Data
{
    public class Product
    {

   
            public int ProductID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int QuantityAvailable { get; set; }
            public string Barcode { get; set; }

            // Foreign key
            public int TenantID { get; set; }

            // Navigation properties
            public Tenant Tenant { get; set; }
            public ICollection<SaleItem> SaleItems { get; set; }
            public ICollection<InventoryLog> InventoryLogs { get; set; }
        }

    
}
