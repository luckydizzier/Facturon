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
                if (Invoice?.IsGrossBased == true)
                {
                    return Quantity * UnitPrice / (1 + TaxRateValue / 100m);
                }
                return Quantity * UnitPrice;
            }
        }

        [NotMapped]
        public decimal GrossAmount
        {
            get
            {
                if (Invoice?.IsGrossBased == true)
                {
                    return Quantity * UnitPrice;
                }
                return Quantity * UnitPrice * (1 + TaxRateValue / 100m);
            }
        }
    }
}
