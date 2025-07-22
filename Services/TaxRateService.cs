using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;
using Facturon.Repositories;

namespace Facturon.Services
{
    public class TaxRateService : ITaxRateService, IEntityService<TaxRate>
    {
        private readonly ITaxRateRepository _taxRateRepository;
        private readonly IProductRepository _productRepository;

        public TaxRateService(ITaxRateRepository taxRateRepository, IProductRepository productRepository)
        {
            _taxRateRepository = taxRateRepository;
            _productRepository = productRepository;
        }

        public async Task<TaxRate?> GetByIdAsync(int id)
        {
            return await _taxRateRepository.GetByIdAsync(id);
        }

        public async Task<List<TaxRate>> GetAllAsync()
        {
            return await _taxRateRepository.GetAllAsync();
        }

        public async Task<Result> CreateAsync(TaxRate rate)
        {
            rate.DateCreated = DateTime.UtcNow;
            rate.DateUpdated = DateTime.UtcNow;
            rate.Active = true;

            await _taxRateRepository.AddAsync(rate);
            return Result.Ok();
        }

        public async Task<Result> UpdateAsync(TaxRate rate)
        {
            var existing = await _taxRateRepository.GetByIdAsync(rate.Id);
            if (existing == null || !existing.Active)
                return Result.Fail("Tax rate not found");

            rate.DateCreated = existing.DateCreated;
            rate.DateUpdated = DateTime.UtcNow;
            rate.Active = existing.Active;

            await _taxRateRepository.UpdateAsync(rate);
            return Result.Ok();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var rate = await _taxRateRepository.GetByIdAsync(id);
            if (rate == null || !rate.Active)
                return Result.Fail("Tax rate not found");

            var products = await _productRepository.GetByConditionAsync(p => p.TaxRateId == id);
            if (products.Count > 0)
                return Result.Fail("Cannot delete tax rate in use");

            await _taxRateRepository.DeleteAsync(id);
            return Result.Ok();
        }

        async Task<IEnumerable<TaxRate>> IEntityService<TaxRate>.GetAllAsync()
        {
            return await GetAllAsync();
        }

        async Task<TaxRate> IEntityService<TaxRate>.CreateAsync(TaxRate entity)
        {
            await _taxRateRepository.AddAsync(entity);
            return entity;
        }
    }
}
