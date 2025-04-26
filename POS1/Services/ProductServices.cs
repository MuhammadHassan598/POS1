using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using POS1.Components.Pages.Products;
using POS1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 

namespace POS1.Services
{
    public class ProductServices
    {
        private readonly StockServices _stockServices;
        private readonly InventoryLogServices _inventoryLogServices;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public ProductServices(IDbContextFactory<ApplicationDbContext> contextFactory, InventoryLogServices inventoryLogServices, StockServices stockServices)
        {
            _contextFactory = contextFactory;
            _inventoryLogServices = inventoryLogServices;
            _stockServices = stockServices;
            
        }

        // Method to retrieve all products
        public async Task<List<Product>> GetAllProductsAsync()
        {
            await using var _context = _contextFactory.CreateDbContext();

            return await _context.Products.ToListAsync();
        }

        // Method to retrieve active products by its  tenant id ID
        public async Task<List<Product>> GetActiveProductsByTenantIdAsync(int tenantId)
        {
            await using var _context = _contextFactory.CreateDbContext();

            return await _context.Products
                  .Include(p => p.ProductType)
                  .Where(p => p.TenantId == tenantId && p.IsActive == true)
                  .ToListAsync();
        }
        // Method to retrieve Inactive products by its  tenant id ID
        public async Task<List<Product>> GetInActiveProductsByTenantIdAsync(int tenantId)
        {
            await using var _context = _contextFactory.CreateDbContext();

            return await _context.Products
                  .Include(p => p.ProductType)
                  .Where(p => p.TenantId == tenantId && p.IsActive == false)
                  .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id,int tenantId)
        {
            await using var _context = _contextFactory.CreateDbContext();

            return _context.Products.Include(p => p.ProductType).FirstOrDefault(s=>s.Id==id && s.TenantId==tenantId);
        }
        // Method to retrieve a product by its ID
        public async Task<Product> GetProductByIdAsync(int id)
        {
            await using var _context = _contextFactory.CreateDbContext();

            return await _context.Products.FindAsync(id);
        }

        // Method to create a new product
        public async Task<Product> CreateProductAsync(Product product, POS1.Data.User user)
        {
            using (var context = _contextFactory.CreateDbContext())
            {

                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var existingProduct = await context.Products
               .FirstOrDefaultAsync(p => p.Name == product.Name && p.TenantId == product.TenantId && p.QuantityUnit==product.QuantityUnit && p.ProductType.Id==product.ProductTypeId);

                        if (existingProduct != null)
                        {
                            throw new Exception("Product with the same name already exists.");
                        }

                        // Add the new product
                        context.Products.Add(product);
                        await context.SaveChangesAsync();

                        // Create an initial stock entry with 0 quantity
                        var initialStock = new Stock
                        {
                            ProductId = product.Id,
                            QuantityAvailable = 0,
                            CostPrice = 0,
                            SalePrice = 0,
                            TenantId=user.TenantID,
                        };

                        context.Stocks.Add(initialStock);
                     
                            await context.SaveChangesAsync();
                        
                        
                         

                        var logEntry = new InventoryLog
                        {
                            ProductId = product.Id,
                            QuantityChanged = 0,
                            ActionIs =         InventoryAction.ProductAdded,
                            LogDateTime = DateTime.Now,
                            UserId = user.UserID,
                            TenantId = user.TenantID,
                        };
                        context.InventoryLogs.Add(logEntry);
                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return product;
                    }
                    catch (Exception)
                    {

                        await transaction.RollbackAsync();
                        throw new Exception("Something went wrong.Please try again! ");  
                    }
                }
            }
        }
        // Method to update an existing product

        public async Task<Product> UpdateProductAsync(Product updatedProduct, POS1.Data.User user)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Retrieve the existing product to update
                        var existingProduct = await context.Products.FindAsync(updatedProduct.Id);
                        if (existingProduct == null)
                        {
                            throw new Exception("Product not found.");
                        }

                        // Update product properties
                        existingProduct.Name = updatedProduct.Name;
                        existingProduct.Description = updatedProduct.Description;
                        existingProduct.QuantityUnit = updatedProduct.QuantityUnit;
                        existingProduct.Barcode = updatedProduct.Barcode;
                        existingProduct.IsActive = updatedProduct.IsActive;
                        existingProduct.ProductTypeId = updatedProduct.ProductTypeId;

                        // Mark the existing product as modified
                        context.Entry(existingProduct).State = EntityState.Modified;
                        await context.SaveChangesAsync();

                        // Create a log entry for the updated product
                        var logEntry = new InventoryLog
                        {
                            ProductId = existingProduct.Id,
                            QuantityChanged = 0, 
                            ActionIs = InventoryAction.ProductUpdated,
                            LogDateTime = DateTime.Now,
                            UserId = user.UserID,
                            TenantId = user.TenantID,
                        };

                        // Add the log entry to the context
                        context.InventoryLogs.Add(logEntry);
                        await context.SaveChangesAsync();

                        await transaction.CommitAsync(); // Commit the transaction
                        return existingProduct; // Return the updated product
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync(); // Rollback in case of error
                        throw; // Re-throw the exception
                    }
                }
            }
        }


        // Method to delete a product
        public async Task<bool> DeleteProductAsync(Product product, POS1.Data.User user)
        {
            await using var _context = _contextFactory.CreateDbContext();

            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User not found");
            }

           var stocks= await _stockServices.GetStocksByIdAsync(product.Id, product.TenantId);
            foreach (var stock in stocks)
            {     
            stock.QuantityAvailable = 0;
           ;
                _context.Entry(stock).State = EntityState.Modified; // Mark as modified
            }

            var totalQuantityAvailable = stocks.Sum(s => s.QuantityAvailable);
            InventoryAction action = InventoryAction.ProductDeleted;
            var logEntry = await _inventoryLogServices.AddLogInventory(product, user, totalQuantityAvailable, action);
            if (logEntry == null)
            {
                throw new InvalidOperationException("Failed to create inventory log entry");
            }
            var existingProduct = await _context.Products.FindAsync(product.Id);
            if (existingProduct == null)
            {
                _context.Products.Attach(product);
              product.IsActive = false;
                _context.Entry(product).State = EntityState.Modified;
            }
            else
            {
          
                existingProduct.IsActive = false;
            }
            try
            {

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving changes: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                throw;
            }
            return true;
        }





        // Method to Un delete a product
        public async Task<bool> ActiveDeleteProductAsync(Product product, POS1.Data.User user)
        {
            await using var _context = _contextFactory.CreateDbContext();

            if (product == null)
            {
               throw new ArgumentException("Product not found");
            }
          if (user == null)
           {
               throw new ArgumentNullException(nameof(user), "User not found");
            }
           InventoryAction action = InventoryAction.ProductUnDeleted;
            var logEntry = await _inventoryLogServices.AddLogInventory(product, user, 0, action);
           if (logEntry == null)
            {
                throw new InvalidOperationException("Failed to create inventory log entry");
            }
            var existingProduct = await _context.Products.FindAsync(product.Id);
            if (existingProduct == null)
            {
                _context.Products.Attach(product);

                product.IsActive = true;
                _context.Entry(product).State = EntityState.Modified;
           }
            else
           {

                existingProduct.IsActive = true;
            }
            try
            {

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving changes: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                throw;
            }
            return true;
        }
        //public async Task<bool> UpdateProductStock(List<Product> updatedProducts)
        //{
        //    await using var _context = _contextFactory.CreateDbContext();

        //    foreach (var upproduct in updatedProducts)
        //    {
        //        var oldprod = await GetProductByIdAsync(upproduct.Id);
        //        oldprod.QuantityAvailable += upproduct.QuantityAvailable;
        //        if (oldprod.Price < upproduct.Price)
        //        {
        //            oldprod.Price = upproduct.Price;
        //            InventoryAction action = InventoryAction.StockUpdated;
        //            //await _inventoryLogServices.LogInventoryChange(oldprod, upproduct.QuantityAvailable, action);
        //        }

        //    }
        //    await _context.SaveChangesAsync();

        //    return true;

        //}


    }
}
