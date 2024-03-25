namespace POS1.Data
{
    public class Payment
    {

      
            public int PaymentID { get; set; }
            public int SaleID { get; set; }
            public decimal AmountPaid { get; set; }
            public DateTime PaymentDateTime { get; set; }
            public string PaymentMethod { get; set; }

            // Foreign key
            public int TenantID { get; set; }

            // Navigation properties
            public Sale Sale { get; set; }
            public Tenant Tenant { get; set; }
        }


 }
