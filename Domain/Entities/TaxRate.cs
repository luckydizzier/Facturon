using System;
using System.Collections.Generic;

namespace Facturon.Domain.Entities
{
    public class TaxRate : BaseEntity
    {
        public required string Code { get; set; }
        public decimal Value { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public virtual List<Product> Products { get; set; } = new();
    }
}
