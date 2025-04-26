namespace POS1.Data
{
    public class Sale
    {
      
            public int SaleID { get; set; }
            public int UserID { get; set; }
            public DateTime SaleDateTime { get; set; }
        public double Discount { get; set; }
        public double TotalAmount { get; set; }

            // Foreign key
            public int TenantID { get; set; }

            // Navigation properties
            public User User { get; set; }
            public Tenant Tenant { get; set; }
            public ICollection<SaleItem> SaleItems { get; set; }
            public ICollection<Payment> Payments { get; set; }
        }



    
}
