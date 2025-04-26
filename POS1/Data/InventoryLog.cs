using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS1.Data
{
    public class InventoryLog
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public InventoryAction ActionIs { get; set; }
        public double QuantityChanged { get; set; }
        public DateTime LogDateTime { get; set; }

        public int UserId { get; set; } // Updated to int
        public int TenantId{ get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public Tenant Tenant { get; set; }
        public User User { get; set; }
    }

    public enum InventoryAction
    {
        ProductAdded,
        ProductUpdated,
        ProductDeleted,
        ProductUnDeleted,
        StockUpdated,
        StockAdded,
        StockReturn,
        StockRemoved,
         Sale,
        SaleReturn,
    }
}
