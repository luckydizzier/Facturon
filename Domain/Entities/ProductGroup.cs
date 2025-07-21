using System.Collections.Generic;

namespace Facturon.Domain.Entities
{
    public class ProductGroup : BaseEntity
    {
        public string Name { get; set; }

        public virtual List<Product> Products { get; set; } = new();
    }
}
