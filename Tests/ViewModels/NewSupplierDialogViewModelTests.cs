using System.Collections.Generic;
using Facturon.App.ViewModels;
using Facturon.Domain.Entities;
using Facturon.Services;
using Moq;
using Xunit;

namespace Facturon.Tests.ViewModels
{
    public class NewSupplierDialogViewModelTests
    {
        private static NewSupplierDialogViewModel CreateVm(IEnumerable<Supplier>? existing = null)
        {
            var service = new Mock<ISupplierService>();
            service.Setup(s => s.GetAllAsync())
                   .ReturnsAsync(existing != null ? new List<Supplier>(existing) : new List<Supplier>());
            return new NewSupplierDialogViewModel(service.Object);
        }

        [Fact]
        public void Name_Empty_ReturnsRequired()
        {
            var vm = CreateVm();
            vm.Name = "";
            Assert.Equal("Required", vm[nameof(NewSupplierDialogViewModel.Name)]);
        }

        [Fact]
        public void Name_Duplicate_ReturnsExists()
        {
            var existing = new Supplier { Name = "Dup", TaxNumber = "1", Address = "A" };
            var vm = CreateVm(new[] { existing });
            vm.Name = "Dup";
            Assert.Equal("Name exists", vm[nameof(NewSupplierDialogViewModel.Name)]);
        }
    }
}
