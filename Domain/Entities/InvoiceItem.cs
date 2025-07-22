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

        /// <summary>
        /// VAT percentage captured at the moment of invoicing.
        /// Stored explicitly to keep historical tax values.
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public decimal TaxRateValue { get; set; }

        public required virtual Invoice Invoice { get; set; }
        public required virtual Product Product { get; set; }

        [NotMapped]
        public decimal NetAmount => Quantity * UnitPrice;

        [NotMapped]
        public decimal GrossAmount
        {
            get
            {
                return NetAmount * (1 + TaxRateValue / 100m);
            }
        }
    }
}
