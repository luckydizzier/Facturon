using System.Collections.Generic;

namespace Facturon.Domain.Entities
{
    public class Unit : BaseEntity
    {
        public required string Name { get; set; }
        public required string ShortName { get; set; }

        public virtual List<Product> Products { get; set; } = new();
    }
}
