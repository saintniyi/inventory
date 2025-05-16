using System.Net.Http;


namespace InventoryAPI.Data
{
    using InventoryData.Data;
    using InventoryModel.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    namespace InventoryAPI.Data
    {
        public class SeedData
        {
            private readonly AppDbContext _context;
            private readonly ILogger<SeedData> _logger;

            public SeedData(AppDbContext context, ILogger<SeedData> logger)
            {
                _context = context;
                _logger = logger;
            }


            public async Task InitializeAsync()
            {
                try
                {
                    await SeedSuppliersAsync();
                    await SeedProductsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }


            private async Task SeedSuppliersAsync()
            {
                if (await _context.Suppliers.AnyAsync()) return;

                _logger.LogInformation("Seeding Suppliers...");

                var suppliers = new[]
                {
                    new Supplier { Name = "Acme Corp", Email = "info@acme.com", Phone = "123-456-7890" },
                    new Supplier { Name = "Global Traders", Email = "contact@global.com", Phone = "234-567-8901" },
                    new Supplier { Name = "Nova Supplies", Email = "hello@nova.com", Phone = "345-678-9012" },
                    new Supplier { Name = "Prime Distributors", Email = "sales@prime.com", Phone = "456-789-0123" },
                    new Supplier { Name = "Eco World", Email = "support@eco.com", Phone = "567-890-1234" }
                };

                //var suppliers1 = new List<Supplier>();
                //suppliers1.Add(new Supplier { Name = "Acme Corp", Email = "info@acme.com", Phone = "123-456-7890" });
                //suppliers1.Add(new Supplier { Name = "Global Traders", Email = "contact@global.com", Phone = "234-567-8901" });
                //suppliers1.Add(new Supplier { Name = "Nova Supplies", Email = "hello@nova.com", Phone = "345-678-9012" });
                //suppliers1.Add(new Supplier { Name = "Prime Distributors", Email = "sales@prime.com", Phone = "456-789-0123" });
                //suppliers1.Add(new Supplier { Name = "Eco World", Email = "support@eco.com", Phone = "567-890-1234" });


                //var suppliers2 = new List<Supplier>()
                //{
                //    new Supplier { Name = "Acme Corp", Email = "info@acme.com", Phone = "123-456-7890" },
                //    new Supplier { Name = "Global Traders", Email = "contact@global.com", Phone = "234-567-8901" },
                //    new Supplier { Name = "Nova Supplies", Email = "hello@nova.com", Phone = "345-678-9012" },
                //    new Supplier { Name = "Prime Distributors", Email = "sales@prime.com", Phone = "456-789-0123" },
                //    new Supplier { Name = "Eco World", Email = "support@eco.com", Phone = "567-890-1234" }
                //};


                await _context.Suppliers.AddRangeAsync(suppliers);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Suppliers seeded successfully.");
            }




            private async Task SeedProductsAsync()
            {
                if (await _context.Products.AnyAsync()) return;

                var supplierIds = await _context.Suppliers.Select(s => s.Id).ToListAsync();
                if (supplierIds.Count < 5)
                {
                    _logger.LogWarning("Not enough suppliers found to seed products.");
                    return;
                }

                _logger.LogInformation("Seeding Products...");

                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Smartphone X200",
                        Category = ProductCategory.Electronics,
                        Price = 799.99m,
                        StockQty = 50,
                        SupplierId = supplierIds[0],
                        ProductImage = Array.Empty<byte>()
                    },
                    new Product
                    {
                        Name = "Men's Jacket",
                        Category = ProductCategory.Apparel,
                        Price = 129.50m,
                        StockQty = 30,
                        SupplierId = supplierIds[1],
                        ProductImage = Array.Empty<byte>()
                    },
                    new Product
                    {
                        Name = "Organic Rice (1kg)",
                        Category = ProductCategory.Groceries,
                        Price = 5.99m,
                        StockQty = 100,
                        SupplierId = supplierIds[2],
                        ProductImage = Array.Empty<byte>()
                    },
                    new Product
                    {
                        Name = "Modern Office Chair",
                        Category = ProductCategory.Furniture,
                        Price = 149.99m,
                        StockQty = 20,
                        SupplierId = supplierIds[3],
                        ProductImage = Array.Empty<byte>()
                    },
                    new Product
                    {
                        Name = "The Clean Coder - Book",
                        Category = ProductCategory.Books,
                        Price = 39.99m,
                        StockQty = 40,
                        SupplierId = supplierIds[4],
                        ProductImage = Array.Empty<byte>()
                    }
                };

                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Products seeded successfully.");
            }




        }
    }





}
