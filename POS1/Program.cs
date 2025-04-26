using Microsoft.AspNetCore.Components.Server;
using POS1.Components;
using POS1.Data;
using POS1.Services;
using POS1.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Radzen; 

using Microsoft.Extensions.DependencyInjection.Extensions;
var builder = WebApplication.CreateBuilder(args);



 
 

builder.Services.AddAuthentication("Identity.Application").AddCookie();
// Register the DbContextFactory
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add services to the container.

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
 
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped< UserServices>();
builder.Services.AddScoped<ProductServices>();
builder.Services.AddScoped<StockServices>();
builder.Services.AddScoped<SaleServices>();
builder.Services.AddScoped<InventoryLogServices>();
builder.Services.AddScoped<ProductTypeServices>();
builder.Services.AddScoped<SupplierServices>();
builder.Services.AddScoped<SupplierAccountServices>();
builder.Services.AddScoped <SupplierBillsServices>();
builder.Services.AddScoped<SupplierItemsServices>();
builder.Services.AddScoped<SupplierPaymentServices>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<BillReferenceServices>();
builder.Services.AddScoped<CustomerAccountService>();
builder.Services.AddScoped<SalePaymentServices>();
 



builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorComponents(options =>
    options.DetailedErrors = builder.Environment.IsDevelopment());

 

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
     
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
 
app.Run();