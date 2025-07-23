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
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IConfirmationDialogService _confirmationService;
        private readonly INewEntityDialogService<PaymentMethod> _paymentMethodDialogService;
        private readonly MainViewModel _mainViewModel;

        public PaymentMethodSelectorViewModel PaymentMethodSelector { get; }

        public InvoiceDetailViewModel(
            IInvoiceService invoiceService,
            IPaymentMethodService paymentMethodService,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<PaymentMethod> paymentMethodDialogService,
            MainViewModel mainViewModel)
        {
            _invoiceService = invoiceService;
            _paymentMethodService = paymentMethodService;
            _confirmationService = confirmationService;
            _paymentMethodDialogService = paymentMethodDialogService;
            _mainViewModel = mainViewModel;

            PaymentMethodSelector = new PaymentMethodSelectorViewModel(
                _paymentMethodService,
                _confirmationService,
                _paymentMethodDialogService);
            PaymentMethodSelector.InitializeAsync().GetAwaiter().GetResult();
            PaymentMethodSelector.PropertyChanged += PaymentMethodSelectorOnPropertyChanged;

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
                    PaymentMethodSelector.SelectedItem = _invoice?.PaymentMethod;
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
            PaymentMethodSelector.SelectedItem = full?.PaymentMethod;
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

        private void PaymentMethodSelectorOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PaymentMethodSelector.SelectedItem) && Invoice != null && PaymentMethodSelector.SelectedItem != null)
            {
                Invoice.PaymentMethod = PaymentMethodSelector.SelectedItem;
                Invoice.PaymentMethodId = PaymentMethodSelector.SelectedItem.Id;
            }
        }

        // TODO: Toggle DetailVisible when invoice selection changes
    }
}
