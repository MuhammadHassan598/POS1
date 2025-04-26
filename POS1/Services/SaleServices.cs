using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using POS1.Data;
using static POS1.Components.Pages.Dashboard.Dashboard;

namespace POS1.Services
{
    public class SaleServices
    {
        private readonly UserServices userServices;
        private readonly InventoryLogServices _inventoryLogServices;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SaleServices(IDbContextFactory<ApplicationDbContext> contextFactory ,InventoryLogServices inventoryLogServices, UserServices userservices)
        {
      
             
             
                _contextFactory = contextFactory;
            _inventoryLogServices = inventoryLogServices;
            userServices=userservices;
        }



     public async Task<Sale> GenerateSale(List<SaleItem> saleItems, POS1.Data.User user,double discount)
         {
           if (saleItems == null || !saleItems.Any())
               throw new ArgumentException("Sale items cannot be empty.", nameof(saleItems));

            if (user == null)
               throw new ArgumentNullException(nameof(user), "Something went wrong.Please try again!");
           
           await using var _context = _contextFactory.CreateDbContext();
 
 
            var sale = new Sale
            {
                UserID = user.UserID,
                TenantID = user.TenantID,
                SaleDateTime = DateTime.Now,
               Discount = discount,
               TotalAmount = saleItems.Sum(item => item.TotalPrice)-discount,
                SaleItems = new List<SaleItem>()
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
  
            try
            {
               _context.Sales.Add(sale);
                await _context.SaveChangesAsync(); 

                foreach (var item in saleItems)
                {
                   if (item == null)
                       throw new ArgumentException("Sale item cannot be empty.");
                    item.SaleID = sale.SaleID; 
              
                
                    var logEntry = new InventoryLog
                    {
                        ProductId = item.ProductID,
                        QuantityChanged = -item.Quantity,
                        ActionIs = InventoryAction.Sale,
                        LogDateTime = DateTime.Now,
                        UserId = user.UserID,
                        TenantId = user.TenantID,
                    };
                    _context.InventoryLogs.Add(logEntry);
                    await _context.SaveChangesAsync();
                    if (logEntry == null)
                   {
                       throw new InvalidOperationException("Something went wrong.Please try again!");
                   }



                   item.stock.QuantityAvailable =Math.Round( item.stock.QuantityAvailable - item.Quantity
                 ,2) ;
                    _context.Stocks.Update(item.stock);
                    var product2 = item.Product;
                  var stock2=  item.stock;
                item.Product = null;
                    item.stock = null;
                    _context.SaleItems.Add(item);
                    item.Product = product2;
                    item.stock = stock2;

                }

              await _context.SaveChangesAsync();
             await transaction.CommitAsync();
        }
          catch
        {
              await transaction.RollbackAsync();
               throw;
           }

           return sale;
        }



       public async Task<Sale> GetSaleByIdAsync(int saleId,int tenantId)
        {
            await using var _context = _contextFactory.CreateDbContext();

            try
            {
 
               var sale = await _context.Sales
                    .Include(s=>s.Tenant)
                    .Include(s => s.SaleItems)
                        .ThenInclude(si => si.Product)  
                    .FirstOrDefaultAsync(s => s.SaleID == saleId&& s.TenantID==tenantId);

    //            if (sale != null)
    //           {
    //                foreach (var saleItem in sale.SaleItems)
    //                {
                        
    //                    var product = await _context.Products.FindAsync(saleItem.ProductID);
    //                    if (product != null)
    //                    {
    //                        saleItem.Product = product;  
    //                    }
    //                   else
    //                    {
    //                                Console.WriteLine($"Product with ID {saleItem.ProductID} not found for SaleItem {saleItem.SaleItemID}");
    //}
    //                }
    //            } 
                return sale;
            }
            catch (Exception ex)
            {
                  Console.WriteLine($"Error retrieving sale with ID {saleId}: {ex.Message}");
                throw;
            }
        }



        public async Task<List<Sale>> GetAllSalesByTenantIdAsync(int tenantId)
        {
           await using var _context = _contextFactory.CreateDbContext();

            try
            {
               
                var sales = await _context.Sales
                    .Where(s => s.TenantID == tenantId)
                    .Include(s => s.SaleItems)
                       .ThenInclude(si => si.Product) 
                    .OrderByDescending(s => s.SaleDateTime) 
                    .ToListAsync();

                return sales;
           }
            catch (Exception ex)
            {
                 Console.WriteLine($"Error retrieving all sales for Tenant ID {tenantId}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<DataItem>> GetAllSalesByTenantIdAsyncLast30Days(int tenantId)
        {
            await using var _context = _contextFactory.CreateDbContext();
            DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);
            DateTime today = DateTime.Now.Date;  

            try
            {
                 var last30Days = Enumerable
                    .Range(0, 30)
                    .Select(i => today.AddDays(-i)) 
                    .ToList();

                   var salesData = await _context.Sales
                    .Where(s => s.TenantID == tenantId && s.SaleDateTime >= thirtyDaysAgo)
                    .GroupBy(s => s.SaleDateTime.Date)
                    .Select(group => new DataItem
                    {
                        Day = group.Key.ToString("dd-MM-yyyy"),
                        TotalAmount = group.Sum(sale => sale.TotalAmount)
                    })
                    .ToListAsync();

                  var result = last30Days
                    .GroupJoin(salesData,
                               date => date.ToString("dd-MM-yyyy"),
                               sale => sale.Day,
                               (date, sales) => new DataItem
                               {
                                   Day = date.ToString("dd-MM"),
                                   TotalAmount = sales.FirstOrDefault()?.TotalAmount ?? 0 // Default to 0 if no sales
                               })
                    .OrderBy(item => DateTime.ParseExact(item.Day, "dd-MM", CultureInfo.InvariantCulture)).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving sales for Tenant ID {tenantId} for the last 30 days: {ex.Message}");
                throw;
            }
        }

        public async Task<List<YearlyDataItem>> GetAllSalesByTenantIdAsyncLastYear(int tenantId)
        {
            await using var _context = _contextFactory.CreateDbContext();
            DateTime lastYear = DateTime.UtcNow.AddYears(-1); // Get the date one year ago in UTC
            DateTime today = DateTime.UtcNow;

            try
            {
                // Generate the last 12 months
                var last12Months = Enumerable
                    .Range(0, 12)
                    .Select(i => new DateTime(today.Year, today.Month, 1).AddMonths(-i))
                    .ToList();

                // Get sales data for the last year, grouped by year and month
                var salesData = await _context.Sales
                    .Where(s => s.TenantID == tenantId && s.SaleDateTime >= lastYear)
                    .GroupBy(s => new { s.SaleDateTime.Year, s.SaleDateTime.Month })
                    .Select(group => new YearlyDataItem
                    {
                        Month = new DateTime(group.Key.Year, group.Key.Month, 1).ToString("MMM yyyy"),
                        TotalAmount = group.Sum(sale => sale.TotalAmount)
                    })
                    .ToListAsync();

                // Left join the sales data with the last 12 months to ensure no missing months
                var result = last12Months
                    .GroupJoin(salesData,
                               date => date.ToString("MMM yyyy"),
                               sale => sale.Month,
                               (date, sales) => new YearlyDataItem
                               {
                                   Month = date.ToString("MMM yyyy"),
                                   TotalAmount = sales.FirstOrDefault()?.TotalAmount ?? 0 // Default to 0 if no sales
                               })
                    .OrderBy(item => DateTime.ParseExact(item.Month, "MMM yyyy", CultureInfo.InvariantCulture))
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving sales for Tenant ID {tenantId} for the last year: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ProcessSaleReturnAsync(List<SaleItem> saleItems,POS1.Data.User user, int saleId,double totalAmount)
        {
            await using var _context = _contextFactory.CreateDbContext();
   await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var reference=await _context.BillRefernces.FirstOrDefaultAsync(s => s.SaleId == saleId && s.TenantId == user.TenantID);
              
                 var sale = await _context.Sales .FirstOrDefaultAsync(s => s.SaleID == saleId);

                if (sale == null)
                {
                    return false;
                }

                foreach (var item in saleItems)
                {

                    var logEntry = new InventoryLog
                    {
                        ProductId = item.ProductID,
                        QuantityChanged = +item.Quantity,
                        ActionIs = InventoryAction.SaleReturn,
                        LogDateTime = DateTime.Now,
                        UserId = user.UserID,
                        TenantId = user.TenantID,
                    };
                     _context.InventoryLogs.Add(logEntry);

                    item.Quantity = -item.Quantity;
                    item.TotalPrice = -item.TotalPrice;
                   item.SaleID = sale.SaleID; 
                    item.TenantID = sale.TenantID;

              var existStock= _context.Stocks.FirstOrDefault(s => s.ID == item.StockId);
                    existStock.QuantityAvailable = Math.Round(existStock.QuantityAvailable + item.Quantity, 2);

                    var productBackup = item.Product;
                    item.Product = null;
                    _context.SaleItems.Add(item); 
                    item.Product = productBackup;

                }

                await _context.SaveChangesAsync();
                if (reference != null)
                {
                    var customerAccount = await _context.CustomerAccounts.FirstOrDefaultAsync(c => c.CustomerId == reference.CustomerId && c.TenantID == reference.TenantId);
              
                    customerAccount.TotalAmountBill=Math.Round(customerAccount.TotalAmountBill - totalAmount, 2);

                    customerAccount.RemainingAmount = Math.Round(customerAccount.RemainingAmount - totalAmount, 2);
                    _context.CustomerAccounts.Update(customerAccount);

                }
                else
                {
                    var payment = new Payment
                    {
                        SaleId =saleId,
                        PaymentMethod=PaymentMethod.CashPayment,
                        PaymentDate =DateTime.Now,
                        AmountPaid=totalAmount,
                        TenantId=user.TenantID,
                    };
                    _context.Payments.Add(payment);
                }
                // Update the total amount of the sale
                sale.TotalAmount =Math.Round( sale.TotalAmount - totalAmount,2);

                _context.Entry(sale).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                 await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Sale> UpdateSaleAsync(Sale sale, List<SaleItem> saleItems,double discount)
        {
           await using var _context = _contextFactory.CreateDbContext();
            await using var transaction = await _context.Database.BeginTransactionAsync();
        var user=  await userServices.GetUserByUserIdAsync(sale.UserID);



            try
            {
                var reference = await _context.BillRefernces.FirstOrDefaultAsync(s => s.SaleId == sale.SaleID && s.TenantId == user.TenantID);

                double TotalDiscount = sale.Discount + discount; 

            foreach (var saleItem in saleItems)
                {

                   
                    var logEntry = new InventoryLog
                    {
                        ProductId = saleItem.ProductID,
                        QuantityChanged = -saleItem.Quantity,
                        ActionIs = InventoryAction.Sale,
                        LogDateTime = DateTime.Now,
                        UserId = user.UserID,
                        TenantId = user.TenantID,
                    };

                 
                    saleItem.SaleID = sale.SaleID;



                    saleItem.stock.QuantityAvailable -= saleItem.Quantity;
                      _context.Stocks.Update(saleItem.stock);   

                    

                    var product2 = saleItem.Product;
                    var stock2 = saleItem.stock;
                    saleItem.Product = null;
                    saleItem.stock = null;
                    _context.SaleItems.Add(saleItem);
                    saleItem.Product = product2;
                    saleItem.stock = stock2;
                }

                await _context.SaveChangesAsync();

                var existingSale = await _context.Sales
                   .Include(s => s.SaleItems)
                  .FirstOrDefaultAsync(s => s.SaleID == sale.SaleID);

                if (existingSale == null)
                {
                    throw new InvalidOperationException($"Sale with ID {sale.SaleID} not found.");
                }


                existingSale.Discount = TotalDiscount;
                // Update the total amount of the sale
                existingSale.TotalAmount = Math.Round( existingSale.SaleItems.Sum(si => si.TotalPrice)-TotalDiscount,2);

                if (reference != null)
                {
                    var customerAccount = await _context.CustomerAccounts.FirstOrDefaultAsync(c => c.CustomerId == reference.CustomerId && c.TenantID == reference.TenantId);

                    customerAccount.TotalAmountBill = Math.Round(customerAccount.TotalAmountBill + existingSale.TotalAmount, 2);

                    customerAccount.RemainingAmount = Math.Round(customerAccount.RemainingAmount + existingSale.TotalAmount, 2);
                    _context.CustomerAccounts.Update(customerAccount);

                }


                await _context.SaveChangesAsync();
               await transaction.CommitAsync();

                return existingSale;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public string FormatPrice(double price)
        {
            string priceWithTwoDecimals = price.ToString("0.00");
            double formattedDouble = double.Parse(priceWithTwoDecimals);
            var culture = new System.Globalization.CultureInfo("ur-PK");
             string formattedPrice = formattedDouble.ToString("C", culture);

                 if (price < 0)
            {
                // Remove the currency symbol, trim spaces, and place the minus sign after the currency symbol
                formattedPrice = formattedPrice.Replace(culture.NumberFormat.CurrencySymbol, "").Trim();
                formattedPrice = $"{culture.NumberFormat.CurrencySymbol} {formattedPrice}";
            }

            return formattedPrice;
        }

        public string FormatSaleDateTime(DateTime saleDateTime)
        {
            return saleDateTime.ToString("dd/MM/yyyy hh:mm tt"); // "tt" for AM/PM indication
        }
        public string FormatDouble(double value)
        {
            return value.ToString("0.00"); // Format value to 2 decimal places
        }
      
    }
}