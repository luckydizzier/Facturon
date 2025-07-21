using System.Collections.Generic;

namespace Facturon.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public int UnitId { get; set; }
        public int ProductGroupId { get; set; }
        public int TaxRateId { get; set; }

        public virtual Unit Unit { get; set; }
        public virtual ProductGroup ProductGroup { get; set; }
        public virtual TaxRate TaxRate { get; set; }
        public virtual List<InvoiceItem> InvoiceItems { get; set; } = new();
    }
}
