using Microsoft.EntityFrameworkCore;
using POS1.Data;

namespace POS1.Services
{
    public class CustomerAccountService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public CustomerAccountService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<CustomerAccount> GetCustomerAccountByIdAsync(int Id, int tenantid)
        {
            await using var _context = _contextFactory.CreateDbContext();
            return await _context.CustomerAccounts.FirstOrDefaultAsync(sa => sa.CustomerId == Id && sa.TenantID == tenantid);

        }

    }
}
