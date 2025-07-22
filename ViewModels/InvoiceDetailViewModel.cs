using System.Windows;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class InvoiceDetailViewModel : BaseViewModel
    {
        private readonly IInvoiceService _invoiceService;
        private readonly MainViewModel _mainViewModel;
        public EditableComboWithAddViewModel<Supplier> SupplierPickerVM { get; }
        public EditableComboWithAddViewModel<Product> ProductPickerVM { get; }
        public EditableComboWithAddViewModel<Unit> UnitPickerVM { get; }
        public EditableComboWithAddViewModel<TaxRate> TaxRatePickerVM { get; }
        public EditableComboWithAddViewModel<ProductGroup> ProductGroupPickerVM { get; }

        public InvoiceDetailViewModel(
            IInvoiceService invoiceService,
            MainViewModel mainViewModel,
            IEntityService<Supplier> supplierService,
            IEntityService<Product> productService,
            IEntityService<Unit> unitService,
            IEntityService<TaxRate> taxRateService,
            IEntityService<ProductGroup> productGroupService)
        {
            _invoiceService = invoiceService;
            _mainViewModel = mainViewModel;
            SupplierPickerVM = new EditableComboWithAddViewModel<Supplier>(supplierService);
            ProductPickerVM = new EditableComboWithAddViewModel<Product>(productService);
            UnitPickerVM = new EditableComboWithAddViewModel<Unit>(unitService);
            TaxRatePickerVM = new EditableComboWithAddViewModel<TaxRate>(taxRateService);
            ProductGroupPickerVM = new EditableComboWithAddViewModel<ProductGroup>(productGroupService);
            InvoiceItems = new ObservableCollection<InvoiceItemViewModel>();
            _mainViewModel.PropertyChanged += MainViewModel_PropertyChanged;
        }

        private Invoice? _invoice;
        public Invoice? Invoice
        {
            get => _invoice;
            set
            {
                if (_invoice != value)
                {
                    _invoice = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(InvoiceItems));
                    OnPropertyChanged(nameof(IsGrossBased));
                }
            }
        }

        public bool IsGrossBased
        {
            get => Invoice?.IsGrossBased ?? false;
            set
            {
                if (Invoice != null && Invoice.IsGrossBased != value)
                {
                    Invoice.IsGrossBased = value;
                    OnPropertyChanged();
                    foreach (var item in InvoiceItems)
                        item.RecalculateAmounts(value);
                    _ = RecalculateTotalsAsync();
                }
            }
        }

        private Visibility _detailVisible = Visibility.Collapsed;
        public Visibility DetailVisible
        {
            get => _detailVisible;
            set
            {
                if (_detailVisible != value)
                {
                    _detailVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<InvoiceItemViewModel> _invoiceItems;
        public ObservableCollection<InvoiceItemViewModel> InvoiceItems
        {
            get => _invoiceItems;
            private set
            {
                if (_invoiceItems != value)
                {
                    _invoiceItems = value;
                    OnPropertyChanged();
                }
            }
        }

        private InvoiceTotals _invoiceTotals = new InvoiceTotals();
        public InvoiceTotals InvoiceTotals
        {
            get => _invoiceTotals;
            private set
            {
                if (_invoiceTotals != value)
                {
                    _invoiceTotals = value;
                    OnPropertyChanged();
                }
            }
        }

        private async void MainViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.SelectedInvoice))
                await LoadInvoiceAsync();
        }

        private async Task LoadInvoiceAsync()
        {
            if (_mainViewModel.SelectedInvoice == null)
            {
                Invoice = null;
                InvoiceItems.Clear();
                InvoiceTotals = new InvoiceTotals();
                return;
            }

            var full = await _invoiceService.GetByIdAsync(_mainViewModel.SelectedInvoice.Id);
            Invoice = full;
            InvoiceItems = new ObservableCollection<InvoiceItemViewModel>(
                full?.Items.Select(i => new InvoiceItemViewModel(i)) ?? new List<InvoiceItemViewModel>());
            foreach (var item in InvoiceItems)
                item.RecalculateAmounts(IsGrossBased);
            OnPropertyChanged(nameof(IsGrossBased));

            if (full != null)
            {
                var totals = await _invoiceService.CalculateTotalsAsync(full);
                InvoiceTotals = new InvoiceTotals
                {
                    TotalNet = Math.Round(totals.TotalNet, 2),
                    TotalVat = Math.Round(totals.TotalVat, 2),
                    TotalGross = Math.Round(totals.TotalGross, 2),
                    ByTaxRate = totals.ByTaxRate
                        .Select(tg => new TaxRateTotal
                        {
                            TaxCode = tg.TaxCode,
                            Net = Math.Round(tg.Net, 2),
                            Vat = Math.Round(tg.Vat, 2),
                            Gross = Math.Round(tg.Gross, 2)
                        })
                        .ToList()
                };
            }
            else
            {
                InvoiceTotals = new InvoiceTotals();
            }
        }

        private async Task RecalculateTotalsAsync()
        {
            if (Invoice == null)
                return;

            var totals = await _invoiceService.CalculateTotalsAsync(Invoice);
            InvoiceTotals = new InvoiceTotals
            {
                TotalNet = Math.Round(totals.TotalNet, 2),
                TotalVat = Math.Round(totals.TotalVat, 2),
                TotalGross = Math.Round(totals.TotalGross, 2),
                ByTaxRate = totals.ByTaxRate
                    .Select(tg => new TaxRateTotal
                    {
                        TaxCode = tg.TaxCode,
                        Net = Math.Round(tg.Net, 2),
                        Vat = Math.Round(tg.Vat, 2),
                        Gross = Math.Round(tg.Gross, 2)
                    })
                    .ToList()
            };
            OnPropertyChanged(nameof(InvoiceItems));
        }

        // TODO: Toggle DetailVisible when invoice selection changes
    }
}
