using Microsoft.EntityFrameworkCore;
using POS1.Data;

namespace POS1.Services
{
    public class SupplierPaymentServices
    {
        private readonly SupplierAccountServices _supplierAccountService;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SupplierPaymentServices(IDbContextFactory<ApplicationDbContext> contextFactory, SupplierAccountServices supplierAccountService)
        {
            _contextFactory = contextFactory;
            _supplierAccountService = supplierAccountService;
        }

      


        public async Task <bool>AddPaymentAsync(SupplierPayment supplierPayment, SupplierAccount supplierAccount)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                   supplierPayment.SuppliersAccountId = supplierAccount.Id;
                supplierPayment.TenantID = supplierAccount.TenantID;
            
                context.SupplierPayments.Add(supplierPayment);


                var existingSupplierAccount = await context.SupplierAccounts
             .FirstOrDefaultAsync(sa => sa.SupplierId == supplierAccount.SupplierId && sa.TenantID == supplierAccount.TenantID);

                if (existingSupplierAccount == null)
                {
                    throw new InvalidOperationException("Supplier account not found.");
                }

                // Update the tracked entity instance

                existingSupplierAccount.TotalAmountPaid += Math.Round(supplierPayment.AmountPaid, 2);
                existingSupplierAccount.RemainingAmount -= Math.Round(supplierPayment.AmountPaid, 2);

                context.SupplierAccounts.Update(existingSupplierAccount);



                await context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if anything fails
                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction rolled back. Error: {ex.Message}");
                return false;
            }
        }








    }
}
