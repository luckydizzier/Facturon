using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facturon.Domain.Entities;
using Facturon.Repositories;

namespace Facturon.Services
{
    public class ProductService : IProductService, IEntityService<Product>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IProductGroupRepository _productGroupRepository;
        private readonly ITaxRateRepository _taxRateRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public ProductService(
            IProductRepository productRepository,
            IUnitRepository unitRepository,
            IProductGroupRepository productGroupRepository,
            ITaxRateRepository taxRateRepository,
            IInvoiceRepository invoiceRepository)
        {
            _productRepository = productRepository;
            _unitRepository = unitRepository;
            _productGroupRepository = productGroupRepository;
            _taxRateRepository = taxRateRepository;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Result> CreateAsync(Product product)
        {
            if (!await ValidateRelations(product))
                return Result.Fail("Invalid relations");

            product.DateCreated = DateTime.UtcNow;
            product.DateUpdated = DateTime.UtcNow;
            product.Active = true;

            await _productRepository.AddAsync(product);
            return Result.Ok();
        }

        public async Task<Result> UpdateAsync(Product product)
        {
            var existing = await _productRepository.GetByIdAsync(product.Id);
            if (existing == null || !existing.Active)
                return Result.Fail("Product not found");

            if (!await ValidateRelations(product))
                return Result.Fail("Invalid relations");

            product.DateCreated = existing.DateCreated;
            product.DateUpdated = DateTime.UtcNow;
            product.Active = existing.Active;

            await _productRepository.UpdateAsync(product);
            return Result.Ok();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null || !product.Active)
                return Result.Fail("Product not found");

            var inInvoices = await _invoiceRepository.GetByConditionAsync(i => i.Items.Any(it => it.ProductId == id));
            if (inInvoices.Count > 0)
                return Result.Fail("Cannot delete product in use");

            await _productRepository.DeleteAsync(id);
            return Result.Ok();
        }

        private async Task<bool> ValidateRelations(Product product)
        {
            var unit = await _unitRepository.GetByIdAsync(product.UnitId);
            if (unit == null || !unit.Active) return false;

            var group = await _productGroupRepository.GetByIdAsync(product.ProductGroupId);
            if (group == null || !group.Active) return false;

            var taxRate = await _taxRateRepository.GetByIdAsync(product.TaxRateId);
            if (taxRate == null || !taxRate.Active) return false;

            return true;
        }

        async Task<IEnumerable<Product>> IEntityService<Product>.GetAllAsync()
        {
            return await GetAllAsync();
        }

        async Task<Product> IEntityService<Product>.CreateAsync(Product entity)
        {
            await _productRepository.AddAsync(entity);
            return entity;
        }
    }
}
