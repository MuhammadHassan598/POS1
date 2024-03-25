namespace POS1.Data
{
    public class SaleItem
    {

            public int SaleItemID { get; set; }
            public int SaleID { get; set; }
            public int ProductID { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; }

            // Foreign key
            public int TenantID { get; set; }

            // Navigation properties
            public Sale Sale { get; set; }
            public Product Product { get; set; }
            public Tenant Tenant { get; set; }
        }


    
}
