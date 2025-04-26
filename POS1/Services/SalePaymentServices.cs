using Microsoft.EntityFrameworkCore;
using POS1.Data;

namespace POS1.Services
{
    public class SalePaymentServices
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SalePaymentServices(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<List<Payment>> GetPaymentsByAccount(int customerId, int tenantId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

         var customerAccount  = context.CustomerAccounts.FirstOrDefault(ca=>ca.CustomerId==customerId && ca.TenantID==tenantId);
            return context.Payments.Where(s => s.CustomerAccountId == customerAccount.Id && s.TenantId == tenantId).ToList();
        }

        public async Task<List<Payment>> GetPaymentsBySale(int saleId, int tenantId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return context.Payments.Where(s => s.SaleId == saleId && s.TenantId == tenantId).ToList();
        }

        public async Task<bool> AddPaymentBySale(Payment payment, Sale sale)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {                     
                context.Payments.Add(payment);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction rolled back. Error: {ex.Message}");
                return false;
            }
      
        }


        public async Task<bool> AddPaymentByAccount(Payment payment, CustomerAccount account)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                context.Payments.Add(payment);
            

                account.TotalAmountPaid = Math.Round(account.TotalAmountPaid + payment.AmountPaid);
                    account.RemainingAmount=Math.Round(account.RemainingAmount - payment.AmountPaid);
                context.CustomerAccounts.Update(account);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction rolled back. Error: {ex.Message}");
                return false;
            }

        }

    }
}
