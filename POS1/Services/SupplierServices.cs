using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using POS1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace POS1.Services
{
    public class SupplierServices
    {


        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SupplierServices(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }



        public async Task<List<Supplier>> GetAllSuppliersByTenentId(int tenantId)
        {
            await using var _context = _contextFactory.CreateDbContext();

            return await _context.Suppliers
                     .Where(s => s.TenantId == tenantId)
                     .ToListAsync();
        }


        public async Task<Supplier> GetSupplierByIdAsync(int id)
        {
            await using var _context = _contextFactory.CreateDbContext();

            return await _context.Suppliers
                     .Where(s => s.Id == id)
                     .FirstOrDefaultAsync();
        }
        public async Task<bool> AddSupplier(Supplier supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException("supplier should'nt be empty!");
            }

            await using var _context = _contextFactory.CreateDbContext();

            var existingSupplier = await _context.Suppliers
      .FirstOrDefaultAsync(s => s.Name == supplier.Name && s.TenantId == supplier.TenantId);

            if (existingSupplier != null)
            {
                // A supplier with the same name already exists for this tenant
                throw new InvalidOperationException("A supplier with the same name already exists.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Suppliers.Add(supplier);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    // Create a new SupplierAccount after registering the supplier
                    var supplierAccount = new SupplierAccount
                    {
                        SupplierId = supplier.Id,
                        TenantID = supplier.TenantId,
                        TotalAmountOwed = 0,
                        TotalAmountPaid = 0,
                        RemainingAmount = 0,
                    
                        //SupplierBills = new List<SuppliersBill>(), // 
                        //SupplierItems = new List<SupplierItem>()   // 
                    };

                    _context.SupplierAccounts.Add(supplierAccount);
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
               throw;
            }
            return true;
        }












    }


}
