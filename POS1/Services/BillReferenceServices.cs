using Microsoft.EntityFrameworkCore;
using POS1.Data;

namespace POS1.Services
{
    public class BillReferenceServices
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public BillReferenceServices(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<BillRefernce>> GetAllBillRefernceBYCustomerId(int Id, int tenantId)
        {
            await using var _context = _contextFactory.CreateDbContext();

            return await _context.BillRefernces.Where(s => s.CustomerId == Id && s.TenantId==tenantId).Include(s=>s.Sale).ToListAsync();
        }

        public async Task<BillRefernce> GetBillRefernceBYSaleId(int Id, int tenantId)
        {
            await using var _context = _contextFactory.CreateDbContext();

            return await _context.BillRefernces.Include(s => s.Customer) .FirstOrDefaultAsync(s => s.SaleId == Id && s.TenantId == tenantId);
        }

        public async Task<BillRefernce> AddBillRefernce(BillRefernce bill,Sale sale)
        {
            await using var _context = _contextFactory.CreateDbContext();


            var customerAccount = await _context.CustomerAccounts
       .FirstOrDefaultAsync(c => c.CustomerId == bill.CustomerId && c.TenantID == bill.TenantId);


            if (customerAccount == null)
            {
                throw new Exception("Customer account not found.");
            }
            customerAccount.TotalAmountBill= Math.Round(customerAccount.TotalAmountBill + sale.TotalAmount);
            customerAccount.RemainingAmount = Math.Round(customerAccount.RemainingAmount + sale.TotalAmount);
            _context.CustomerAccounts.Update(customerAccount);
            // Add the new bill reference
            _context.BillRefernces.Add(bill);
            await _context.SaveChangesAsync();

            return bill;
        }


    }
}
