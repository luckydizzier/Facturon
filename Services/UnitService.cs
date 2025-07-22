using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;
using Facturon.Repositories;

namespace Facturon.Services
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IProductRepository _productRepository;

        public UnitService(IUnitRepository unitRepository, IProductRepository productRepository)
        {
            _unitRepository = unitRepository;
            _productRepository = productRepository;
        }

        public async Task<Unit?> GetByIdAsync(int id)
        {
            return await _unitRepository.GetByIdAsync(id);
        }

        public async Task<List<Unit>> GetAllAsync()
        {
            return await _unitRepository.GetAllAsync();
        }

        public async Task<Result> CreateAsync(Unit unit)
        {
            unit.DateCreated = DateTime.UtcNow;
            unit.DateUpdated = DateTime.UtcNow;
            unit.Active = true;

            await _unitRepository.AddAsync(unit);
            return Result.Ok();
        }

        public async Task<Result> UpdateAsync(Unit unit)
        {
            var existing = await _unitRepository.GetByIdAsync(unit.Id);
            if (existing == null || !existing.Active)
                return Result.Fail("Unit not found");

            unit.DateCreated = existing.DateCreated;
            unit.DateUpdated = DateTime.UtcNow;
            unit.Active = existing.Active;

            await _unitRepository.UpdateAsync(unit);
            return Result.Ok();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var unit = await _unitRepository.GetByIdAsync(id);
            if (unit == null || !unit.Active)
                return Result.Fail("Unit not found");

            var products = await _productRepository.GetByConditionAsync(p => p.UnitId == id);
            if (products.Count > 0)
                return Result.Fail("Cannot delete unit in use");

            await _unitRepository.DeleteAsync(id);
            return Result.Ok();
        }
    }
}
