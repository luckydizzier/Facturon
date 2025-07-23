using System;
using System.Threading.Tasks;
using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class TaxRateSelectorViewModel : EditableComboWithAddViewModel<TaxRate>
    {
        private readonly ITaxRateService _taxRateService;

        public TaxRateSelectorViewModel(
            ITaxRateService service,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<TaxRate> dialogService)
            : base(service, confirmationService, dialogService)
        {
            _taxRateService = service;
        }

        public async Task InitializeAsync(DateTime date)
        {
            var items = await _taxRateService.GetActiveForDateAsync(date);
            Items.Clear();
            foreach (var item in items)
                Items.Add(item);
        }
    }
}

