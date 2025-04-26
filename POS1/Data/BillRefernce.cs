namespace POS1.Data
{
    public class BillRefernce
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; }
        public string? number { get; set; }
        public int SaleId { get; set; }
        public Customer Customer { get; set; }
        public Sale Sale { get; set; }
        public Tenant Tenant { get; set; }
    }
}
