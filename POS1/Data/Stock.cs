namespace POS1.Data
{
    public class Stock
    {
        public int ID { get; set; }
  
        public double CostPrice
        { get; set; }

   
        public double SalePrice
        { get; set; }
        public double TotalQuantity{ get; set; }
        public double QuantityAvailable { get; set; }
     
       
        public int ProductId { get; set; }
        public int TenantId { get; set; }

        public Product Product { get; set; }
        public Tenant Tenant { get; set; }

    }
    public enum QuantityUnit
    {
        Pcs,
        Kilogram,
        Liter,
        Meter,
        Set,
        Feet
    }
}
