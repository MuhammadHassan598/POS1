using Microsoft.EntityFrameworkCore;
using POS1.Components.Pages.Sale;
using POS1.Data;
using System;
using System.Net.Sockets;

namespace POS1.Services
{
    public class SupplierBillsServices
    {
        private readonly SupplierItemsServices _supplierItemsServices;
        private readonly SupplierAccountServices _supplierAccountService;
        private readonly UserServices userServices;
        private readonly InventoryLogServices _inventoryLogServices;

        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SupplierBillsServices(IDbContextFactory<ApplicationDbContext> contextFactory, SupplierItemsServices supplierItemsServices
, SupplierAccountServices supplierAccountService, InventoryLogServices inventoryLogServices, UserServices userservices)
        {
            _contextFactory = contextFactory;
            _inventoryLogServices = inventoryLogServices;
            userServices = userservices;
            _supplierAccountService = supplierAccountService;
            _supplierItemsServices = supplierItemsServices;
        }


        public async Task<List<SuppliersBill>> GetSupplierBillsBySupplierId(int supplierId, int tenantId)
        {
            await using var _context = _contextFactory.CreateDbContext();
            var bills = await _context.SuppliersBills
         .Where(sb => sb.SupplierId == supplierId && sb.TenantID == tenantId)
         .ToListAsync();
            return bills;
        }

        public async Task<SuppliersBill> GeSupplierBillByIdAsync(int id, int tenantId)
        {
            await using var _context = _contextFactory.CreateDbContext();

            var bill = await _context.SuppliersBills
              .Include(sb => sb.Tenant)
              .Include(sb => sb.Supplier)
              .Include(sb => sb.SupplierItems).ThenInclude(si => si.Product)
              .Where(sb => sb.Id == id && sb.TenantID == tenantId)
              .FirstOrDefaultAsync();



            return bill;
        }



        public async Task<SuppliersBill> CreateSupplierBill(int supplierId, List<SupplierItem> supplierItems, double totalAmount, POS1.Data.User user)
        {
             

            await using var _context = _contextFactory.CreateDbContext();
            var suppliersAccount = await _supplierAccountService.GetSupplierAccountByIdAsync(supplierId, user.TenantID);

            var supplierBill = new SuppliersBill
            {
                SupplierId = supplierId,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                TenantID = user.TenantID,
                SupplierItems = new List<SupplierItem>()
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                
                await _context.SuppliersBills.AddAsync(supplierBill);
                await _context.SaveChangesAsync();  



           
                foreach (var item in supplierItems)
                {


                    var newStock=item.Product.Stocks.FirstOrDefault();


                    newStock.TotalQuantity = item.QuantityReceived;
                    newStock.QuantityAvailable = item.QuantityReceived;
                    newStock.CostPrice = item.UnitPrice;
                    
                     



                    item.SuppliersBillID = supplierBill.Id; // 
                    item.TenantID = user.TenantID;
                    item.Id = 0;
                    var product=item.Product;
                    
                    item.Product = null;
                    _context.SupplierItems.Add(item);
                    await _context.SaveChangesAsync();
                     

                    item.Product = product;
  
                    var logEntry = new InventoryLog
                    {
                        ProductId = item.ProductId,
                        QuantityChanged = item.QuantityReceived,
                        ActionIs = InventoryAction.StockAdded,
                        LogDateTime = DateTime.Now,
                        UserId = user.UserID,
                        TenantId = user.TenantID,
                    };
                    _context.InventoryLogs.Add(logEntry);
                    await _context.SaveChangesAsync();





                }

                // Update suppliers account
                suppliersAccount.TotalAmountOwed = Math.Round(suppliersAccount.TotalAmountOwed + supplierBill.TotalAmount, 2);
                suppliersAccount.RemainingAmount = Math.Round(suppliersAccount.RemainingAmount + supplierBill.TotalAmount, 2);
                suppliersAccount.TotalAmountPaid += 0;

                _context.SupplierAccounts.Update(suppliersAccount);

                await _context.SaveChangesAsync(); // Final save for all changes
               
                await transaction.CommitAsync();

                return supplierBill;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction rolled back. Error: {ex.Message}");
                throw;
            }
        }


        public async Task<SuppliersBill> UpdateSupplierBill(SuppliersBill supplierBill, List<SupplierItem> supplierItems, double totalAmount, POS1.Data.User user)
        {


            await using var _context = _contextFactory.CreateDbContext();
            var suppliersAccount = await _supplierAccountService.GetSupplierAccountByIdAsync(supplierBill.SupplierId, user.TenantID);

         

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                double quantity;
                double totalExactAmount = 0;
                foreach (var item in supplierItems)
                {
                    var stockset = item.Product.Stocks.FirstOrDefault().SalePrice;
                    var isExist = await _context.SupplierItems.AsNoTracking().FirstOrDefaultAsync(si => si.SuppliersBillID == supplierBill.Id && si.Id == item.Id);


                    if (isExist == null)
                    {
                        totalExactAmount=Math.Round( totalExactAmount +  item.TotalPrice, 2); 
              
                        item.SuppliersBillID = supplierBill.Id;
                        item.TenantID = user.TenantID;
                        item.Id = 0;
                        var product = item.Product;
                        _context.Entry(product).State = EntityState.Detached;

                        item.Product = null;
                        _context.SupplierItems.Add(item);
                        await _context.SaveChangesAsync();
                        item.Product = product;
                        quantity = item.QuantityReceived;
                        var initialStock = new Stock
                        {
                            ProductId = item.ProductId,
                            TotalQuantity = item.QuantityReceived,
                            QuantityAvailable = item.QuantityReceived,
                            CostPrice = item.UnitPrice,
                            SalePrice = stockset,
                            TenantId = user.TenantID,
                        };

                        _context.Stocks.Add(initialStock);


                    }

                    else
                    {
                        totalExactAmount = Math.Round(totalExactAmount+(item.TotalPrice - isExist.TotalPrice),2);
                        quantity = Math.Round(item.QuantityReceived - isExist.QuantityReceived,2);

                        var newStock = item.Product.Stocks.FirstOrDefault();


                        newStock.TotalQuantity = item.QuantityReceived;
                        newStock.QuantityAvailable = item.QuantityReceived;
                    
                        newStock.CostPrice = item.UnitPrice;
                        _context.Stocks.Update(newStock);
                        _context.SupplierItems.Update(item);
                           
                        

                    }
                 


                    var logEntry = new InventoryLog
                    {
                        ProductId = item.ProductId,
                        QuantityChanged = quantity,
                        ActionIs = InventoryAction.StockUpdated,
                        LogDateTime = DateTime.Now,
                        UserId = user.UserID,
                        TenantId = user.TenantID,
                    };
                    _context.InventoryLogs.Add(logEntry);
                    await _context.SaveChangesAsync();
                     

                }

                supplierBill.TotalAmount=Math.Round( supplierBill.TotalAmount + totalExactAmount,2);
                _context.SuppliersBills.Update(supplierBill);
                await _context.SaveChangesAsync();

                // Update suppliers account
                suppliersAccount.TotalAmountOwed=Math.Round( suppliersAccount.TotalAmountOwed + totalExactAmount, 2);
                suppliersAccount.RemainingAmount= Math.Round(suppliersAccount.RemainingAmount + totalExactAmount, 2);
                suppliersAccount.TotalAmountPaid += 0;

                _context.SupplierAccounts.Update(suppliersAccount);

                await _context.SaveChangesAsync(); // Final save for all changes
                await transaction.CommitAsync();

                return supplierBill;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction rolled back. Error: {ex.Message}");
                throw;
            }
        }


        public async Task<bool> ReturnSupplierBill(List<SupplierItem> items,SuppliersBill bill,POS1.Data.User user,double totalAmount)
        {


            await using var _context = _contextFactory.CreateDbContext();
            var suppliersAccount = await _supplierAccountService.GetSupplierAccountByIdAsync(bill.SupplierId , bill.TenantID);



            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                foreach (var item in items)
                {

                    item.QuantityReceived = -item.QuantityReceived;  
                    item.TotalPrice = -item.TotalPrice;


                    _context.SupplierItems.Add(item);
                    await _context.SaveChangesAsync();
                    var existingStock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.ProductId == item.ProductId && s.TenantId == bill.TenantID && s.CostPrice==item.UnitPrice&&s.QuantityAvailable>= item.QuantityReceived);

                    if (existingStock != null)
                    {

                        existingStock.TotalQuantity = Math.Round(existingStock.TotalQuantity +item.QuantityReceived  ,2);
                        existingStock.QuantityAvailable =Math.Round( existingStock.QuantityAvailable + item.QuantityReceived,2);
                        _context.Stocks.Update(existingStock);
                        await _context.SaveChangesAsync();
 

                    var logEntry = new InventoryLog
                    {
                        ProductId = item.ProductId,
                        QuantityChanged = -item.QuantityReceived,
                        ActionIs = InventoryAction.StockReturn,
                        LogDateTime = DateTime.Now,
                        UserId = user.UserID,
                        TenantId = user.TenantID,
                    };
                    _context.InventoryLogs.Add(logEntry);
                    await _context.SaveChangesAsync();
              }

              bill.TotalAmount =Math.Round(bill.TotalAmount - totalAmount,2);
                _context.SuppliersBills.Update(bill);
                await _context.SaveChangesAsync();

                // Update suppliers account
                suppliersAccount.TotalAmountOwed =Math.Round(suppliersAccount.TotalAmountOwed - totalAmount,2 );
                    suppliersAccount.RemainingAmount = Math.Round(suppliersAccount.RemainingAmount - totalAmount, 2);
                suppliersAccount.TotalAmountPaid += 0;

                _context.SupplierAccounts.Update(suppliersAccount);

                await _context.SaveChangesAsync(); 
                await transaction.CommitAsync();
                   
                }
                return true;
            }
            catch(Exception ex) {
                await transaction.RollbackAsync(); 
                return false;
            }


          

        }



    }
}
