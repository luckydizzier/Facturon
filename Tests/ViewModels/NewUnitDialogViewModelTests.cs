using System.Collections.Generic;
using Facturon.App.ViewModels.Dialogs;
using Facturon.Domain.Entities;
using Facturon.Services;
using Moq;
using Xunit;

namespace Facturon.Tests.ViewModels
{
    public class NewUnitDialogViewModelTests
    {
        private static NewUnitDialogViewModel CreateVm(IEnumerable<Unit>? existing = null)
        {
            var service = new Mock<IUnitService>();
            service.Setup(s => s.GetAllAsync())
                   .ReturnsAsync(existing != null ? new List<Unit>(existing) : new List<Unit>());
            return new NewUnitDialogViewModel(service.Object);
        }

        [Fact]
        public void ShortName_Duplicate_ReturnsExists()
        {
            var existing = new Unit { Name = "U", ShortName = "pcs" };
            var vm = CreateVm(new[] { existing });
            vm.ShortName = "pcs";
            Assert.Equal("Code exists", vm[nameof(NewUnitDialogViewModel.ShortName)]);
        }
    }
}
