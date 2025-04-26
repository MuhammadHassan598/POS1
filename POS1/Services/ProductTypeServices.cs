using Microsoft.EntityFrameworkCore;
using POS1.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS1.Services
{
    public class ProductTypeServices
    {

        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public ProductTypeServices(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }


        public async Task<List<ProductType>> GetAllProductTypesAsync(int tenantId)
        {
           await using var _context = _contextFactory.CreateDbContext();
            return await _context.ProductTypes.Where(pt => pt.TenantId == tenantId).ToListAsync();
        }

        public async Task<ProductType> GetProductTypeByIdAsync(int productTypeId)
        {
            await using var _context = _contextFactory.CreateDbContext();

            return await _context.ProductTypes.FindAsync(productTypeId);
        }

        public async Task<ProductType> CreateProductTypeAsync(ProductType productType)
        {
            await using var _context = _contextFactory.CreateDbContext();

            _context.ProductTypes.Add(productType);
            await _context.SaveChangesAsync();
            return productType;
        }

        public async Task<bool> UpdateProductTypeAsync(ProductType productType)
        {
            await using var _context = _contextFactory.CreateDbContext();

            _context.Entry(productType).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductTypeAsync(int productTypeId)
        {
            await using var _context = _contextFactory.CreateDbContext();

            var productType = await _context.ProductTypes.FindAsync(productTypeId);
            if (productType == null)
                return false;

            _context.ProductTypes.Remove(productType);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
