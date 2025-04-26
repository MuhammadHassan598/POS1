namespace POS1.Data
{
    public class Payment
    {     
           public int  Id { get; set; }
           public int? SaleId { get; set; }
           public int? CustomerAccountId { get; set; }
            
            public PaymentMethod PaymentMethod { get; set; }
            public DateTime PaymentDate { get; set; }
            public double AmountPaid { get; set; }
            public int TenantId { get; set; }
            public string? TransactionId { get; set; }
            public string? PaymentGateway { get; set; }
            public string? BankBranch { get; set; }
            public string? BankName { get; set; }
            public string? CheckNumber { get; set; }
            public Sale Sale { get; set; }
            public CustomerAccount CustomerAccount { get; set; }
            public Tenant Tenant { get; set; }
        }
    public enum PaymentMethod
    {
        CashPayment,
        OnlinePayment,
        CheckPayment
    }

}
