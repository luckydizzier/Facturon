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
            INewEntityDialogService<TaxRate> dialogService,
            ISelectionHistoryService historyService)
            : base(service, confirmationService, dialogService, historyService)
        {
            _taxRateService = service;
        }

        public async Task InitializeAsync(DateTime date)
        {
            Items.Clear();
            var recent = await _taxRateService.GetMostRecentAsync(5);
            foreach (var item in recent.Where(r => r.ValidFrom <= date && r.ValidTo >= date))
                Items.Add(item);
            var items = await _taxRateService.GetActiveForDateAsync(date);
            foreach (var item in items.Where(i => Items.All(r => r.Id != i.Id)))
                Items.Add(item);
        }
    }
}

