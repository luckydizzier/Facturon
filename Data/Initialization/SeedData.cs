using System;
using System.Threading.Tasks;
using Bogus;
using Facturon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Facturon.Data.Initialization
{
    public static class SeedData
    {
        public static async Task InitializeAsync(FacturonDbContext context)
        {
            if (await context.Suppliers.AnyAsync())
                return;

            var faker = new Faker("en");

            var unit = new Unit { Name = "Piece", ShortName = "pcs" };
            var group = new ProductGroup { Name = "General" };
            var taxRate = new TaxRate { Code = "TAX20", Value = 20m, ValidFrom = DateTime.Today };
            var paymentMethod = new PaymentMethod { Name = "Cash" };

            await context.Units.AddAsync(unit);
            await context.ProductGroups.AddAsync(group);
            await context.TaxRates.AddAsync(taxRate);
            await context.PaymentMethods.AddAsync(paymentMethod);
            await context.SaveChangesAsync();

            var product = new Product
            {
                Name = faker.Commerce.Product(),
                UnitId = unit.Id,
                ProductGroupId = group.Id,
                TaxRateId = taxRate.Id,
                Unit = unit,
                ProductGroup = group,
                TaxRate = taxRate
            };
            await context.Products.AddAsync(product);

            var supplier = new Supplier
            {
                Name = faker.Company.CompanyName(),
                TaxNumber = faker.Random.Replace("##-#######"),
                Address = faker.Address.FullAddress()
            };
            await context.Suppliers.AddAsync(supplier);

            await context.SaveChangesAsync();
        }
    }
}
