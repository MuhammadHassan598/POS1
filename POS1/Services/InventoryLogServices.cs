using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using POS1.Data; 
namespace POS1.Services
{
    public class InventoryLogServices
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public InventoryLogServices(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<List<InventoryLog>> GetAllInventoryLogsAsync(int id,int tenantId)
        {
            await using var context = _contextFactory.CreateDbContext();
           
            return await context.InventoryLogs.Where(log => log.ProductId == id && log.TenantId == tenantId).Include(log => log.Product).ToListAsync();
        }

        public async Task<InventoryLog> GetInventoryLogByIdAsync(int inventoryLogId)
        {


            await using var _context = _contextFactory.CreateDbContext();

            // Retrieve a specific inventory log by its ID from the database
            return await _context.InventoryLogs.FindAsync(inventoryLogId);
        }
        public async Task<InventoryLog> AddLogInventory(Product product,User user ,double quantityChange, InventoryAction action)
        {


            if (product == null)
                throw new ArgumentNullException(nameof(product), "Product cannot be null.");

            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
 
            await using var _context = _contextFactory.CreateDbContext();


            var logEntry = new InventoryLog
            {
                ProductId = product.Id,
                LogDateTime = DateTime.UtcNow,
        
                ActionIs = action,
              QuantityChanged = action == InventoryAction.StockRemoved || action == InventoryAction.Sale ? -quantityChange : quantityChange,

            
                UserId = user.UserID,
                TenantId = product.TenantId,

            };

            _context.InventoryLogs.Add(logEntry);
            await _context.SaveChangesAsync();
            return logEntry;
        }

    }
}
