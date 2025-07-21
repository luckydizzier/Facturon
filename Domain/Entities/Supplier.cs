using System.Collections.Generic;

namespace Facturon.Domain.Entities
{
    public class Supplier : BaseEntity
    {
        public required string Name { get; set; }
        public required string TaxNumber { get; set; }
        public required string Address { get; set; }

        public virtual List<Invoice> Invoices { get; set; } = new();
    }
}
