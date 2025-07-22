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

        public required virtual Invoice Invoice { get; set; }
        public required virtual Product Product { get; set; }

        [NotMapped]
        public decimal NetAmount => Quantity * UnitPrice;

        [NotMapped]
        public decimal GrossAmount
        {
            get
            {
                var rate = Product?.TaxRate?.Value ?? 0m;
                return NetAmount * (1 + rate / 100m);
            }
        }
    }
}
