using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Facturon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Facturon.Data.Initialization
{
    public static class SeedData
    {
        public static async Task InitializeAsync(FacturonDbContext context)
        {
            if (await context.Suppliers.AnyAsync())
                return;

            var faker = new Faker("en");

            var units = new[]
            {
                new Unit { Name = "Piece", ShortName = "pcs" },
                new Unit { Name = "Kilogram", ShortName = "kg" }
            };
            var groups = new[]
            {
                new ProductGroup { Name = "General" },
                new ProductGroup { Name = "Food" }
            };
            var taxRates = new[]
            {
                new TaxRate { Code = "TAX20", Value = 20m, ValidFrom = DateTime.Today },
                new TaxRate { Code = "TAX10", Value = 10m, ValidFrom = DateTime.Today }
            };
            var paymentMethods = new[]
            {
                new PaymentMethod { Name = "Cash" },
                new PaymentMethod { Name = "Card" }
            };

            await context.Units.AddRangeAsync(units);
            await context.ProductGroups.AddRangeAsync(groups);
            await context.TaxRates.AddRangeAsync(taxRates);
            await context.PaymentMethods.AddRangeAsync(paymentMethods);
            await context.SaveChangesAsync();

            var products = new List<Product>();
            for (int i = 0; i < 3; i++)
            {
                var unit = faker.PickRandom(units);
                var group = faker.PickRandom(groups);
                var rate = faker.PickRandom(taxRates);

                var product = new Product
                {
                    Name = faker.Commerce.Product(),
                    UnitId = unit.Id,
                    ProductGroupId = group.Id,
                    TaxRateId = rate.Id,
                    Unit = unit,
                    ProductGroup = group,
                    TaxRate = rate
                };
                products.Add(product);
            }
            await context.Products.AddRangeAsync(products);

            var suppliers = new List<Supplier>();
            for (int i = 0; i < 2; i++)
            {
                suppliers.Add(new Supplier
                {
                    Name = faker.Company.CompanyName(),
                    TaxNumber = faker.Random.Replace("##-#######"),
                    Address = faker.Address.FullAddress()
                });
            }
            await context.Suppliers.AddRangeAsync(suppliers);

            await context.SaveChangesAsync();

            var invoices = new List<Invoice>();
            for (int i = 0; i < 2; i++)
            {
                var supplier = faker.PickRandom(suppliers);
                var method = faker.PickRandom(paymentMethods);

                var invoice = new Invoice
                {
                    Date = DateTime.Today.AddDays(-i),
                    Number = $"INV-{i + 1:0000}",
                    Issuer = "FACTURON",
                    SupplierId = supplier.Id,
                    PaymentMethodId = method.Id,
                    Supplier = supplier,
                    PaymentMethod = method
                };

                for (int j = 0; j < 2; j++)
                {
                    var product = faker.PickRandom(products);
                    var qty = faker.Random.Decimal(1, 5);
                    var price = faker.Random.Decimal(10, 100);

                    var item = new InvoiceItem
                    {
                        ProductId = product.Id,
                        Product = product,
                        Invoice = invoice,
                        Quantity = qty,
                        UnitPrice = price,
                        Total = qty * price
                    };
                    invoice.Items.Add(item);
                }

                invoices.Add(invoice);
            }

            await context.Invoices.AddRangeAsync(invoices);
            await context.SaveChangesAsync();
        }

        public static async Task SeedAsync(FacturonDbContext context, ILogger logger)
        {
            logger.LogInformation("Seeding initial data");
            await InitializeAsync(context);
        }
    }
}
