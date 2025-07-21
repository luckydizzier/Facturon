using System.Collections.Generic;

namespace Facturon.Services
{
    public class ValidationResult
    {
        public Dictionary<string, string> Errors { get; } = new();
        public bool IsValid => Errors.Count == 0;

        public void AddError(string field, string message)
        {
            Errors[field] = message;
        }
    }
}
