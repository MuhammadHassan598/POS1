using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
namespace POS1.Data
{


        public class ApplicationDbContext : DbContext
        {
            public IConfiguration _config { get; set; }
            public ApplicationDbContext(IConfiguration config)
            {
                _config = config;
            }
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            }

            public DbSet<Tenant> Tenants { get; set; }
            public DbSet<User> Users { get; set; }
             public DbSet<Product> Products { get; set; }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Sale> Sales { get; set; }
             public DbSet<SaleItem> SaleItems { get; set; }
             public DbSet<Payment> Payments { get; set; }
             public DbSet<InventoryLog> InventoryLogs { get; set; }

        public DbSet<ProductType> ProductTypes { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierAccount> SupplierAccounts { get; set; }
        public DbSet<SupplierItem> SupplierItems { get; set; }
        public DbSet<SuppliersBill> SuppliersBills { get; set; }
        public DbSet<SupplierPayment> SupplierPayments { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerAccount> CustomerAccounts { get; set; }
        public DbSet<BillRefernce> BillRefernces { get; set; }




    }

}
