using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;
using Facturon.Repositories;

namespace Facturon.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public SupplierService(ISupplierRepository supplierRepository, IInvoiceRepository invoiceRepository)
        {
            _supplierRepository = supplierRepository;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _supplierRepository.GetByIdAsync(id);
        }

        public async Task<List<Supplier>> GetAllAsync()
        {
            return await _supplierRepository.GetAllAsync();
        }

        public async Task<Result> CreateAsync(Supplier supplier)
        {
            supplier.DateCreated = DateTime.UtcNow;
            supplier.DateUpdated = DateTime.UtcNow;
            supplier.Active = true;

            await _supplierRepository.AddAsync(supplier);
            return Result.Ok();
        }

        public async Task<Result> UpdateAsync(Supplier supplier)
        {
            var existing = await _supplierRepository.GetByIdAsync(supplier.Id);
            if (existing == null || !existing.Active)
                return Result.Fail("Supplier not found");

            supplier.DateCreated = existing.DateCreated;
            supplier.DateUpdated = DateTime.UtcNow;
            supplier.Active = existing.Active;

            await _supplierRepository.UpdateAsync(supplier);
            return Result.Ok();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null || !supplier.Active)
                return Result.Fail("Supplier not found");

            var invoices = await _invoiceRepository.GetByConditionAsync(i => i.SupplierId == id);
            if (invoices.Count > 0)
                return Result.Fail("Cannot delete supplier with invoices");

            await _supplierRepository.DeleteAsync(id);
            return Result.Ok();
        }
    }
}
