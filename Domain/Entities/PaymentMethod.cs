using System.Collections.Generic;

namespace Facturon.Domain.Entities
{
    public class PaymentMethod : BaseEntity
    {
        public required string Name { get; set; }

        public virtual List<Invoice> Invoices { get; set; } = new();
    }
}
