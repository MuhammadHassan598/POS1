using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace POS1.Data
{
    public class SupplierAccount
    {
        [Key]
        public int Id { get; set; }

        public int SupplierId { get; set; }

       
  
        public double TotalAmountOwed { get; set; }


        public double TotalAmountPaid { get; set; }

        public double RemainingAmount { get; set; }
        public int TenantID { get; set; }

 
        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }

        [ForeignKey("TenantID")]
        public Tenant Tenant { get; set; }


 
    }
}
