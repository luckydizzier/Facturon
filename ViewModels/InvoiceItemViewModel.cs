using System;
using Facturon.Domain.Entities;

namespace Facturon.App.ViewModels
{
    public class InvoiceItemViewModel : BaseViewModel
    {
        public InvoiceItemViewModel(InvoiceItem item)
        {
            Item = item;
            NetAmount = item.NetAmount;
            GrossAmount = item.GrossAmount;
            TaxAmount = item.TaxAmount;
        }

        public InvoiceItem Item { get; }

        public Product Product => Item.Product;
        public decimal Quantity => Item.Quantity;
        public decimal UnitPrice => Item.UnitPrice;
        public decimal TaxRateValue => Item.TaxRateValue;

        private decimal _netAmount;
        public decimal NetAmount
        {
            get => _netAmount;
            private set
            {
                if (_netAmount != value)
                {
                    _netAmount = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _grossAmount;
        public decimal GrossAmount
        {
            get => _grossAmount;
            private set
            {
                if (_grossAmount != value)
                {
                    _grossAmount = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _taxAmount;
        public decimal TaxAmount
        {
            get => _taxAmount;
            private set
            {
                if (_taxAmount != value)
                {
                    _taxAmount = value;
                    OnPropertyChanged();
                }
            }
        }

        public void RecalculateAmounts(bool isGrossBased)
        {
            if (TaxRateValue == 0)
                return;

            if (isGrossBased)
            {
                GrossAmount = Math.Round(Quantity * UnitPrice, 2);
                NetAmount = Math.Round(GrossAmount / (1 + TaxRateValue / 100m), 2);
                TaxAmount = Math.Round(GrossAmount - NetAmount, 2);
            }
            else
            {
                NetAmount = Math.Round(Quantity * UnitPrice, 2);
                TaxAmount = Math.Round(NetAmount * (TaxRateValue / 100m), 2);
                GrossAmount = Math.Round(NetAmount + TaxAmount, 2);
            }
        }
    }
}
