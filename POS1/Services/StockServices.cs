using Microsoft.EntityFrameworkCore;
using POS1.Components.Pages.Products;
using POS1.Data;

namespace POS1.Services
{
    public class StockServices
    {


        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public StockServices(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<List<Stock>> GetStocksByProductIdsAsync(List<int> productIds, int tenantId)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Stocks
             .Where(s => productIds.Contains(s.ProductId) && s.Product.TenantId == tenantId)
             .ToListAsync();


            }




        }

        public async Task<List<Stock>> GetStocksByIdAsync(int productId, int tenantId)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Stocks
                    .Where(s => s.ProductId == productId && s.TenantId == tenantId)
                    .ToListAsync();
            }
        }

        public async Task<Stock> GetSingleStocksByIdAsync(int productId, int tenantId, double unitPrice, double quantity)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Stocks
                    .Where(s => s.ProductId == productId && s.TenantId == tenantId && s.CostPrice == unitPrice  && s.QuantityAvailable == quantity && s.TotalQuantity == quantity)
                    .FirstOrDefaultAsync(); 
            }
        }





    }

}

