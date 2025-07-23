using System.Collections.Generic;

namespace Facturon.Domain.Entities
{
    public class Product : BaseEntity
    {
        public required string Name { get; set; }
        public int UnitId { get; set; }
        public int ProductGroupId { get; set; }
        public int TaxRateId { get; set; }
        public decimal NetUnitPrice { get; set; }

        public required virtual Unit Unit { get; set; }
        public required virtual ProductGroup ProductGroup { get; set; }
        public required virtual TaxRate TaxRate { get; set; }
        public virtual List<InvoiceItem> InvoiceItems { get; set; } = new();
    }
}
