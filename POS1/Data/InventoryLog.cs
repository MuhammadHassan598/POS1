using System.ComponentModel.DataAnnotations;

namespace POS1.Data
{
    public class InventoryLog
    {

        [Key]
        public int LogID { get; set; }
            public int ProductID { get; set; }
            public string ActionIs { get; set; }
            public int QuantityChanged { get; set; }
            public DateTime LogDateTime { get; set; }

            // Foreign key
            public int TenantID { get; set; }

            // Navigation properties
            public Product Product { get; set; }
            public Tenant Tenant { get; set; }
        }


    
}
