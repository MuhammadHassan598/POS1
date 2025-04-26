using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace POS1.Data
{
    public class CustomerAccount
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public double TotalAmountBill { get; set; }
        public double TotalAmountPaid { get; set; }
        public double RemainingAmount { get; set; }
        public int TenantID { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        [ForeignKey("TenantID")]
        public Tenant Tenant { get; set; }
    }
}
