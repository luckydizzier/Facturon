using System;
using System.Collections.Generic;

namespace Facturon.Domain.Entities
{
    public class Invoice : BaseEntity
    {
        public DateTime Date { get; set; }
        public string Number { get; set; }
        public string Issuer { get; set; }
        public int SupplierId { get; set; }
        public int PaymentMethodId { get; set; }

        public virtual Supplier Supplier { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual List<InvoiceItem> Items { get; set; } = new();
    }
}
