using System.Collections.Generic;

namespace Facturon.Services
{
    public class TaxRateTotal
    {
        public string TaxCode { get; set; } = string.Empty;
        public decimal Net { get; set; }
        public decimal Vat { get; set; }
        public decimal Gross { get; set; }
    }

    public class InvoiceTotals
    {
        public decimal TotalNet { get; set; }
        public decimal TotalVat { get; set; }
        public decimal TotalGross { get; set; }
        public List<TaxRateTotal> ByTaxRate { get; set; } = new();
    }
}
