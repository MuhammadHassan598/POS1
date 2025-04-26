using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace POS1.Data
{
    public class SupplierItem
    {
        [Key]
        public int Id { get; set; }

        // Foreign keys
        public int SuppliersBillID { get; set; }
        public int ProductId { get; set; }

        public double QuantityReceived { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }

        // Foreign key to Tenant (assuming TenantID is a foreign key)
        public int TenantID { get; set; }

       


        // Navigation properties

        public SuppliersBill suppliersBill { get; set; }
    
 
       

       
        public Product Product { get; set; }
 
        public Tenant Tenant { get; set; }
    }
}
