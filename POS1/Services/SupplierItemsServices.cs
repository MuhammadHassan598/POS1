using Microsoft.EntityFrameworkCore;
using POS1.Data;

namespace POS1.Services
{
    public class SupplierItemsServices
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SupplierItemsServices(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }


        public async Task<List<SupplierItem>> GetSupplierItemsByIdAsync(int id)
        {
            await using var _context = _contextFactory.CreateDbContext();
            var items = await _context.SupplierItems.Where(sa => sa.SuppliersBillID == id).Include(sa => sa.Product).ToListAsync();

            return items;
        }

        public async Task<SupplierItem>GetSupplierItemByIdAsync(int billId,int id)
        {
            await using var _context = _contextFactory.CreateDbContext();
            var item = await _context.SupplierItems.Where(sa => sa.SuppliersBillID == billId&& sa.Id==id).FirstOrDefaultAsync();

            return item;
        }




        public async Task AddSupplierItems(List<SupplierItem> supplierItems, int supplierBillId, POS1.Data.User user)
        {
            await using var _context = _contextFactory.CreateDbContext();
            // Add supplier items to the database
            foreach (var item in supplierItems)
            {
                item.SuppliersBillID = supplierBillId;
                item.TenantID = user.TenantID;

                _context.SupplierItems.Add(item);
            }

            await _context.SaveChangesAsync();
        }

    }
}
