using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using POS1.Data;

namespace POS1.Services
{
    public class SupplierAccountServices
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SupplierAccountServices(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;

        }
        public async Task<SupplierAccount> GetSupplierAccountByIdAsync(int supplierId, int tenantid)
        {
            await using var _context = _contextFactory.CreateDbContext();
            return await _context.SupplierAccounts.FirstOrDefaultAsync(sa => sa.SupplierId == supplierId && sa.TenantID == tenantid);

        }
        public async Task<bool> UpdateSupplierAccountAsync(SuppliersBill supplierBill)
        {
            if (supplierBill == null)
            {
                throw new ArgumentNullException(nameof(supplierBill), "Supplier bill cannot be null.");
            }

            await using var _context = _contextFactory.CreateDbContext();
            var supplierAccount = await _context.SupplierAccounts.FirstOrDefaultAsync(sa => sa.SupplierId == supplierBill.SupplierId);

            try
            {
                if (supplierAccount == null)
                {
                    throw new InvalidOperationException("please register again");

                }
                else
                {

                    supplierAccount.TotalAmountOwed += supplierBill.TotalAmount;

                    supplierAccount.RemainingAmount += supplierBill.TotalAmount;
                    supplierAccount.TotalAmountPaid += 0;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

       




    }
}
