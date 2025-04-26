using Microsoft.EntityFrameworkCore;
using POS1.Data;

namespace POS1.Services
{
    public class CustomerService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public CustomerService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<List<Customer>> GetAllCustomersByTenentId(int tenantId)
        {
            await using var _context = _contextFactory.CreateDbContext();

            return await _context.Customers.Where(s => s.TenantId == tenantId).ToListAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            await using var _context = _contextFactory.CreateDbContext();

            return await _context.Customers
                     .Where(s => s.Id == id)
                     .FirstOrDefaultAsync();
        }


        public async Task<bool> AddCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException("supplier should'nt be empty!");
            }
            await using var _context = _contextFactory.CreateDbContext();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {     
                var existingCustomer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Name == customer.Name && c.TenantId == customer.TenantId);

                if (existingCustomer != null)
                { 
                    await transaction.RollbackAsync();
                    return false;  
                }


                _context.Customers.Add(customer);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    
                    var customerAccount = new CustomerAccount
                    {
                        CustomerId = customer.Id,
                        TenantID = customer.TenantId,
                        TotalAmountBill = 0,
                        TotalAmountPaid = 0,
                        RemainingAmount = 0,
    };

                    _context.CustomerAccounts.Add(customerAccount);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    return false;
                }


            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return false;
            }
            return true;
        }
    }
}
