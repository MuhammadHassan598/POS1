using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace POS1.Data
{
    public class SupplierPayment
    { 
        [Key]
        public int Id { get; set; }
        public int SuppliersAccountId{ get; set; }
        public PaymentMethods PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }  
        public double AmountPaid { get; set; }  
        public int TenantID { get; set; }
       public string? TransactionId { get; set; }
        public string? PaymentGateway { get; set; } 
        public string? BankBranch { get; set; } 
        public string? BankName { get; set; } 
        public string? CheckNumber { get; set; } 

        [ForeignKey("SuppliersAccountId")]
        public SupplierAccount SupplierAccount { get; set; }
        [ForeignKey("TenantID")]
        public Tenant Tenant { get; set; }
      
    }
    public enum PaymentMethods
    {
        CashPayment,
        OnlinePayment,
        CheckPayment
    }


}

