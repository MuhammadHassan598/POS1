namespace POS1.Data
{
    public class SaleItem
    {

            public int SaleItemID { get; set; }
            public int SaleID { get; set; }
            public int ProductID { get; set; }

        public int StockId { get; set; }
        public double Quantity { get; set; }
        public string QuantityUnit { get; set; }
        public double UnitPrice { get; set; }
            public double TotalPrice { get; set; }
       
            // Foreign key
            public int TenantID { get; set; }
     
        // Navigation properties
        public Sale Sale { get; set; }
            public Product Product { get; set; }
        public Stock stock
        {
            get; set;
        }
            public Tenant Tenant { get; set; }
        }


    
}
