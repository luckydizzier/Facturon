using System.Collections.Generic;

namespace Facturon.Domain.Entities
{
    public class Unit : BaseEntity
    {
        public string Name { get; set; }
        public string ShortName { get; set; }

        public virtual List<Product> Products { get; set; } = new();
    }
}
