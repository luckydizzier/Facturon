using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;
using Facturon.Repositories;

namespace Facturon.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository, IInvoiceRepository invoiceRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<PaymentMethod?> GetByIdAsync(int id)
        {
            return await _paymentMethodRepository.GetByIdAsync(id);
        }

        public async Task<List<PaymentMethod>> GetAllAsync()
        {
            return await _paymentMethodRepository.GetAllAsync();
        }

        public async Task<Result> CreateAsync(PaymentMethod method)
        {
            method.DateCreated = DateTime.UtcNow;
            method.DateUpdated = DateTime.UtcNow;
            method.Active = true;

            await _paymentMethodRepository.AddAsync(method);
            return Result.Ok();
        }

        public async Task<Result> UpdateAsync(PaymentMethod method)
        {
            var existing = await _paymentMethodRepository.GetByIdAsync(method.Id);
            if (existing == null || !existing.Active)
                return Result.Fail("Payment method not found");

            method.DateCreated = existing.DateCreated;
            method.DateUpdated = DateTime.UtcNow;
            method.Active = existing.Active;

            await _paymentMethodRepository.UpdateAsync(method);
            return Result.Ok();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var method = await _paymentMethodRepository.GetByIdAsync(id);
            if (method == null || !method.Active)
                return Result.Fail("Payment method not found");

            var invoices = await _invoiceRepository.GetByConditionAsync(i => i.PaymentMethodId == id);
            if (invoices.Count > 0)
                return Result.Fail("Cannot delete payment method in use");

            await _paymentMethodRepository.DeleteAsync(id);
            return Result.Ok();
        }
    }
}
