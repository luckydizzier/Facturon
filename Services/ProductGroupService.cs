using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;
using Facturon.Repositories;

namespace Facturon.Services
{
    public class ProductGroupService : IProductGroupService
    {
        private readonly IProductGroupRepository _groupRepository;
        private readonly IProductRepository _productRepository;

        public ProductGroupService(IProductGroupRepository groupRepository, IProductRepository productRepository)
        {
            _groupRepository = groupRepository;
            _productRepository = productRepository;
        }

        public async Task<ProductGroup?> GetByIdAsync(int id)
        {
            return await _groupRepository.GetByIdAsync(id);
        }

        public async Task<List<ProductGroup>> GetAllAsync()
        {
            return await _groupRepository.GetAllAsync();
        }

        public async Task<Result> CreateAsync(ProductGroup group)
        {
            group.DateCreated = DateTime.UtcNow;
            group.DateUpdated = DateTime.UtcNow;
            group.Active = true;

            await _groupRepository.AddAsync(group);
            return Result.Ok();
        }

        public async Task<Result> UpdateAsync(ProductGroup group)
        {
            var existing = await _groupRepository.GetByIdAsync(group.Id);
            if (existing == null || !existing.Active)
                return Result.Fail("Product group not found");

            group.DateCreated = existing.DateCreated;
            group.DateUpdated = DateTime.UtcNow;
            group.Active = existing.Active;

            await _groupRepository.UpdateAsync(group);
            return Result.Ok();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var group = await _groupRepository.GetByIdAsync(id);
            if (group == null || !group.Active)
                return Result.Fail("Product group not found");

            var products = await _productRepository.GetByConditionAsync(p => p.ProductGroupId == id);
            if (products.Count > 0)
                return Result.Fail("Cannot delete group in use");

            await _groupRepository.DeleteAsync(id);
            return Result.Ok();
        }
    }
}
