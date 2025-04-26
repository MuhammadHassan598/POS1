using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace POS1.Data
{
    public class SuppliersBill
    {
        [Key]
        public int Id { get; set; }

        // Foreign key to Supplier
        public int SupplierId { get; set; }

        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }

        // Foreign key to Tenant
        public int TenantID { get; set; }
          // Navigation properties
        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }

          public ICollection<SupplierItem> SupplierItems { get; set; }
        [ForeignKey("TenantID")]
        public Tenant Tenant { get; set; }
    }
}
