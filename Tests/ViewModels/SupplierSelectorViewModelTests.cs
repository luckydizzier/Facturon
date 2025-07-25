using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.App.ViewModels;
using Facturon.Domain.Entities;
using Facturon.Services;
using Moq;
using Xunit;

namespace Facturon.Tests.ViewModels
{
    public class SupplierSelectorViewModelTests
    {
        [Fact]
        public async Task InitializeAsync_LoadsRecentFirst()
        {
            var service = new Mock<ISupplierService>();
            var confirm = new Mock<IConfirmationDialogService>();
            var dialog = new Mock<INewEntityDialogService<Supplier>>();
            var history = new Mock<ISelectionHistoryService>();

            var recent = new Supplier { Id = 2, Name = "B" };
            var all = new List<Supplier>
            {
                new Supplier { Id = 1, Name = "A" },
                recent
            };

            service.Setup(s => s.GetMostRecentAsync(It.IsAny<int>())).ReturnsAsync(new List<Supplier> { recent });
            service.Setup(s => s.GetAllAsync()).ReturnsAsync(all);

            var vm = new SupplierSelectorViewModel(service.Object, confirm.Object, dialog.Object, history.Object);
            await vm.InitializeAsync();

            Assert.Equal(recent, vm.Items[0]);
        }
    }
}
