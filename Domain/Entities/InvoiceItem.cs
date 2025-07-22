using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Facturon.Domain.Entities
{
    public class InvoiceItem : BaseEntity
    {
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        public int TaxRateId { get; set; }

        /// <summary>
        /// VAT percentage captured at the moment of invoicing.
        /// Stored explicitly to keep historical tax values.
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public decimal TaxRateValue { get; set; }

        public required virtual Invoice Invoice { get; set; }
        public required virtual Product Product { get; set; }
        public required virtual TaxRate TaxRate { get; set; }

        [NotMapped]
        public decimal NetAmount
        {
            get
            {
                decimal net;
                if (Invoice?.IsGrossBased == true)
                {
                    net = Quantity * UnitPrice / (1 + TaxRateValue / 100m);
                }
                else
                {
                    net = Quantity * UnitPrice;
                }

                return Math.Round(net, 2);
            }
        }

        [NotMapped]
        public decimal GrossAmount
        {
            get
            {
                decimal gross;
                if (Invoice?.IsGrossBased == true)
                {
                    gross = Quantity * UnitPrice;
                }
                else
                {
                    gross = Quantity * UnitPrice * (1 + TaxRateValue / 100m);
                }

                return Math.Round(gross, 2);
            }
        }

        [NotMapped]
        public decimal TaxAmount => Math.Round(GrossAmount - NetAmount, 2);
    }
}
