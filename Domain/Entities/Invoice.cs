using System;
using System.Collections.Generic;

namespace Facturon.Domain.Entities
{
    public class Invoice : BaseEntity
    {
        public DateTime Date { get; set; }
        public required string Number { get; set; }
        public required string Issuer { get; set; }
        public int SupplierId { get; set; }
        public int PaymentMethodId { get; set; }

        /// <summary>
        /// Indicates whether invoice item prices are entered including VAT.
        /// </summary>
        public bool IsGrossBased { get; set; }

        public required virtual Supplier Supplier { get; set; }
        public required virtual PaymentMethod PaymentMethod { get; set; }
        public virtual List<InvoiceItem> Items { get; set; } = new();
    }
}
