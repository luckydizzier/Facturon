# Prompt Log

## [domain_agent] Initialize core domain model files for Facturon
Created initial domain entity classes under `Domain/Entities`.

## [db_agent] Added FacturonDbContext with Fluent API configurations

## [repo_agent] Implement EF Core repositories
Created base repository abstractions and EF Core implementations for Invoice and Product entities under `Repositories/`.
## [repo_agent] Extend repositories with querying and patch support
Added AsQueryable, GetByConditionAsync, PatchAsync to base repository and interfaces. Implemented repositories for Supplier, TaxRate, PaymentMethod, Unit, ProductGroup.

## [service_agent] Implement service layer for all entities
Added Result class and service interfaces/implementations under `Services/` for Invoice, Product, Supplier, PaymentMethod, Unit, ProductGroup, and TaxRate. Includes validation and soft delete logic.
## [service_agent] Extend service layer for invoice items and validation
Implemented InvoiceItemService and validation helpers. Updated InvoiceService totals and creation logic.
## [startup_agent] Configure program startup with DI and Serilog logging
## [startup_agent] Fix missing host project and build infrastructure for Facturon
## [domain_agent] Add Facturon.Domain.csproj to build domain entities
## [db_agent] Add Facturon.Data.csproj referencing Domain and EF Core
## [repo_agent] Add Facturon.Repositories.csproj referencing Data and Domain
## [service_agent] Add Facturon.Services.csproj referencing Repositories
## [domain_agent] Mark non-nullable entity properties as required
Adjusted domain model classes to use the `required` keyword for mandatory
properties and navigation references, addressing compiler warnings.
## [db_agent] Include relational EF Core package
Added `Microsoft.EntityFrameworkCore.Relational` to the Data project to access
Fluent API extensions like `ToTable` and `HasDefaultValueSql`.
## [startup_agent] Reference SQLite provider
Added `Microsoft.EntityFrameworkCore.Sqlite` package to the startup project to
enable `UseSqlite` configuration.

## [db_agent] Fix missing builder using
Added metadata builder using statement to FacturonDbContext to resolve extension methods.
## [orchestrator_agent] Add root solution
Created Facturon.sln including all projects for easier IDE use.

## [service_agent] Fix missing System.Linq using
Added missing using directive in ProductService for LINQ extension methods.
## [startup_agent] Resolve missing host and logging references
Added Microsoft.Extensions.Hosting and Serilog packages to the startup project and updated App.xaml.cs null checks.

## [startup_agent] Fix build entry point and Serilog warning
Removed Program.cs to avoid duplicate entry points and moved host initialization to App.xaml.cs. Updated Serilog.Settings.Configuration to version 7.0.0.
## [ui_agent] Implement initial WPF MVVM UI
Added views, view models, and startup wiring for keyboard-first invoice UI.

## [ui_agent] Fix missing Task using directives
Added System.Threading.Tasks using statements to InvoiceListViewModel and MainViewModel.

## [ui_agent] Set default Supplier and PaymentMethod when creating new invoices
Initialized required navigation properties in MainViewModel to satisfy C# "required" members.
## [migration_agent] Implement database initialization and schema validation
Added DbInitializer with schema checks, startup invocation, and optional seed data using Bogus.

## [db_agent] Resolve Product required properties in seed data
Set Unit, ProductGroup, and TaxRate navigation properties when creating Product in SeedData to satisfy C# required members.
## [db_agent] Implement robust DbInitializer
Created new DbInitializer at Data/DbInitializer.cs with migration checks, schema validation, and optional seeding. Updated startup to use it.
## [db_agent] Ensure database creation when no migrations present
Added a check in DbInitializer to run EnsureCreated when the project has no migrations, preventing missing table errors on first run.
## [db_agent] Fix missing System.Linq using in DbInitializer
## [db_agent] Seed invoices and sample data
Added invoice and invoice item seeding plus additional mock products, suppliers, units, groups, rates, and payment methods.
## [orchestrator_agent] Wire up startup and UI display
Injected MainWindow and MainViewModel via dependency injection, logged UI startup, and simplified MainWindow layout to host InvoiceListView only.
## [ui_agent] Fix MainWindow resource loading and layout
Ensured MainWindow.xaml is compiled as a resource by linking Views in the WPF csproj, added missing code-behind, and restored the view layout with InvoiceListView and InvoiceDetailView bindings.

## [ui_agent] Implement two-pane layout in MainWindow
Reorganized MainWindow.xaml to bind InvoiceListView and InvoiceDetailView with conditional visibility.

## [ui_agent] Add DetailVisible property to InvoiceDetailViewModel
Added visibility property with change notification for toggling the detail view.

## [ui_agent] Fix DetailVisible binding to MainViewModel
Updated binding in MainWindow.xaml to reference DetailVisible from window context via RelativeSource.
## [ui_agent] Fix DataContext binding for DetailVisible
Referenced DataContext.DetailVisible when binding InvoiceDetailView visibility in MainWindow.xaml to resolve WPF error.
## [test_agent] Add InvoiceService unit tests
Created xUnit test project under `Tests` with Moq and initial tests for InvoiceService.
## [ui_agent] Add debug logging and no-data view
## [ui_agent] Add missing view constructors
Added constructors in InvoiceListView and InvoiceDetailView to call InitializeComponent so their XAML content renders.

## [service_agent] Load invoice items in GetByIdAsync
Enhanced InvoiceService.GetByIdAsync to eagerly load invoice items and related
product data with Include/ThenInclude chains.

## [ui_agent] Load selected invoice details
Introduced SelectedInvoice property in MainViewModel and updated
InvoiceDetailViewModel to subscribe to selection changes, load full invoice
details via IInvoiceService, and expose an ObservableCollection for binding.

## [domain_agent] Add computed NetAmount and GrossAmount to InvoiceItem
Implemented readonly properties in InvoiceItem.cs using Product.TaxRate for VAT calculation with null safety. Added NotMapped attributes.

## [domain_agent] Persist tax rate value on InvoiceItem
Added `TaxRateValue` property to InvoiceItem for storing VAT percentage and updated
NetAmount and GrossAmount calculations to use it.

## [db_agent] Configure InvoiceItem.TaxRateValue
Mapped new field in FacturonDbContext with decimal column type and default value
of 0.

## [service_agent] Populate TaxRateValue on creation
Invoice and InvoiceItem services now copy the product's TaxRate value when items
are added or updated. Invoice totals calculation uses the stored value.

## [test_agent] Adapt tests for fixed tax rate
Updated InvoiceServiceTests to set TaxRateValue on invoice items for totals.

## [service_agent] Add GetTotalsAsync and totals rounding support
Implemented new GetTotalsAsync method in InvoiceService and interface. InvoiceTotals now exposes NetTotal, TaxTotal and GrossTotal aliases.

## [ui_agent] Display rounded invoice totals in detail view
InvoiceDetailViewModel loads rounded totals on selection change and InvoiceDetailView shows the values under the item grid.

## [ui_agent] Display Tax Rate in Invoice Items Grid
Added Tax % column to InvoiceDetailView bound to InvoiceItem.TaxRateValue with right-aligned formatting.

## [domain_agent] Add IsGrossBased flag to Invoice
Added boolean property to `Invoice` entity for toggling gross based pricing.

## [db_agent] Map IsGrossBased in DbContext
Configured default value for the new flag in `FacturonDbContext`.

## [ui_agent] Add price mode toggle and formatting fixes
Inserted checkbox bound to `Invoice.IsGrossBased` and applied `StringFormat=F2` to price related columns.

## [service_agent] Handle gross based price calculations
`InvoiceItemService` now converts prices based on invoice mode when creating or updating items.

## [ui_agent] Improve MainWindow interactions
Set focus to invoice list on load and added escape exit confirmation dialog.

## [test_agent] Cover gross/net price logic
New `InvoiceItemServiceTests` verify proper price conversion for gross and net modes.

## [domain_agent] Adjust invoice item amounts for gross/net mode
`InvoiceItem.NetAmount` and `GrossAmount` now derive values based on `Invoice.IsGrossBased`.

## [service_agent] Preserve entered unit price and compute totals
`InvoiceItemService` no longer converts `UnitPrice` but sets `Total` depending on invoice mode. `InvoiceService.CalculateTotalsAsync` also respects `IsGrossBased`.

## [ui_agent] Recalculate totals when price mode toggles
Added `IsGrossBased` property in `InvoiceDetailViewModel` to trigger total recalculation and bound checkbox to it.

## [test_agent] Update tests for gross/net handling
Adjusted invoice item service tests and added gross-based totals test.
## [domain_agent] Round line and total amounts
Implemented rounding for NetAmount, GrossAmount and new TaxAmount properties. Updated totals calculation and added rounding test.
## [ui_agent] VAT breakdown view
Added tax amount column and totals per VAT rate in invoice detail.
## [ui_agent] Navigation tweaks
Default focus on invoice list, arrow navigation for items, and new Esc exit dialog.

## [ui_agent] Dynamic item VAT recalculation
Implemented InvoiceItemViewModel with recomputation logic. InvoiceDetailViewModel now updates item amounts when pricing mode toggles. Added unit tests for RecalculateAmounts.

## [ui_agent] Add automation identifiers for InvoiceDetailView
Added x:Name and AutomationProperties.Name on key controls for Appium tests.

## [test_agent] Add WinAppDriver UI test
Created InvoiceUITests using MSTest and Appium WebDriver.
## [editable-combo-agent] Add generic editable combo components and services
- Introduced IEntityService for generic CRUD operations and updated entity service interfaces.
- Added EditableComboWithAdd user control with accompanying view model.
- Created basic EntityCreateDialog and related view model for inline creation.
- Added interfaces for confirmation and navigation services.


## [service_agent] Replace WPF FocusNavigationDirection
Added local FocusNavigationDirection enum to Services and updated INavigationService to avoid WPF dependency.

## [orchestrator_agent] Implement inline entity creation dialog system
- Added async IConfirmationDialogService interface and WPF implementation.
- Created INewEntityDialogService<T> abstraction and Supplier dialog service.
- Added NewSupplierDialog view and view model with keyboard-friendly bindings.
- Extended EditableComboWithAddViewModel to confirm missing items and create new ones via dialogs.
- Registered new services in Startup.
## [ui_agent] Add unit creation dialog and integrate editable combo
- Created NewUnitDialog view and NewUnitDialogViewModel with validation and keyboard shortcuts.
- Updated EditableComboWithAdd control to trigger confirmation on Enter or focus loss.
- Replaced unit ComboBox in NewProductDialog with EditableComboWithAdd bound to new view model.

## [startup_agent] Register unit dialog service
- Implemented NewUnitDialogService and added DI registration in StartupOrchestrator.
- Extended NewProductDialogService to supply confirmation and new unit dialog dependencies.

## [orchestrator_agent] Wire up inline unit creation
- Modified NewProductDialogViewModel to use EditableComboWithAddViewModel for units.
- New units are saved through dialog service and selected automatically.
## [ui_agent] Implement tax rate creation dialog
- Added NewTaxRateDialog view and view model with validation and keyboard support.
- Created NewTaxRateDialogService and registered it in Startup.
- Updated NewProductDialog to use EditableComboWithAdd for tax rates.
- Extended NewProductDialogService and ViewModel to use the new dialog.
## [ui_agent] Implement product group creation dialog
- Added NewProductGroupDialog view and view model for inline creation.
- Created ProductGroupSelectorViewModel and dialog service.
- Updated NewProductDialog to use editable combo with inline group creation.
- Registered NewProductGroupDialogService in Startup.
## [service_agent] Filter tax rates by date
- Added GetActiveForDateAsync to ITaxRateService and implemented in TaxRateService.
- Created TaxRateSelectorViewModel to load rates valid on a given date.
- Updated NewProductDialogViewModel to initialize tax rate selector with today's date.
## [ui_agent] Implement invoice item input row
- Replaced item editing grid with input row + readonly list. Added InvoiceItemInputViewModel and view, integrated in detail screen.
- Items can be added with Enter and cleared with Escape, Delete removes selected rows with confirmation.

## [ui_agent] Sync detail visibility on selection change
Implemented UpdateDetailVisibility in InvoiceDetailViewModel to set MainViewModel.DetailVisible based on SelectedInvoice and refresh CloseDetailCommand when hidden.
## [ui_agent] Enable invoice item editing
Implemented editing support in InvoiceItemInputViewModel with BeginEdit/CommitEdit. Added UI hooks for F2 or double-click to start editing. Added service UpdateAsync tests and UI test placeholder.
## [orchestrator_agent] Implement Navigation Service
- Added NavigationService with MoveFocus method using Keyboard and TraversalRequest.
- Registered INavigationService in StartupOrchestrator.
- Replaced direct Focus calls in views with NavigationService usage.
## [ui_agent] Theme validation templates
- Added default validation template and DataGrid row styling in LightTheme.xaml.
- Updated all dialog views to bind Validation.Errors for inline messages.
## [doc_agent] Document UX updates
- Created docs/UX.md describing new validation visuals and row styles.
## [orchestrator_agent] Relocate NavigationService
Moved WPF-based NavigationService implementation to Startup project and removed it from Services.
## [ui_agent] Initialize invoice items field
Initialized _invoiceItems collection at declaration in InvoiceDetailViewModel to satisfy nullability analysis.
## [service_agent] Resolve missing namespaces and style error
- Added System.Threading.Tasks using to IConfirmationDialogService.
- Added System using to ITaxRateService.
- Moved AlternationCount setter to DataGrid style in LightTheme.xaml.

## [ui_agent] Clarify DataGrid style property
- Set ItemsControl.AlternationCount in LightTheme.xaml to avoid row style warning.

## [startup_agent] Resolve FocusNavigationDirection ambiguity
- Removed System.Windows.Input using and fully qualified references in NavigationService.


## [startup_agent] Cast Keyboard.FocusedElement
- Cast Keyboard.FocusedElement to UIElement before calling MoveFocus to satisfy .NET focus API.

## [db_agent] Add EF Core migrations support and restructure initializer
- Added Microsoft.EntityFrameworkCore.Design to Data project and included Migrations folder.
- Created initial migration defining tables in dependency order.

## [db_agent] Restore EnsureCreated fallback
- DbInitializer now checks for migrations and calls EnsureCreated when none exist, preventing missing table errors.
## [ui_agent] Add DisplayMemberPath support to EditableComboWithAdd
- Introduced DisplayMemberPath dependency property and bound ComboBox.DisplayMemberPath.
- Specified DisplayMemberPath on dialog views using EditableComboWithAdd.

## [startup_agent] Ensure portable defaults
- StartupOrchestrator now creates `appsettings.json` with an executable-relative
  database connection when missing and loads configuration from that path.
- LoggingConfiguration writes logs under `AppContext.BaseDirectory`.
- Facturon.App.csproj copies the configuration file to the output directory.

## [db_agent] Database path helper
- Added `DbPathHelper.GetConnectionString` returning the SQLite path near the
  executable. Startup uses this when no connection string is configured.

## [doc_agent] Document portable usage
- Added a README section describing automatic generation of configuration,
  database, and logs beside the executable.

## [ui_agent] Add key bindings for item input
- Bound Enter to AddCommand and Escape to ClearCommand in InvoiceItemInputView. Removed PreviewKeyDown handler.

## [ui_agent] Bind item commands in InvoiceDetailView
- Added BeginEditItemCommand and InputBindings for Delete, F2 and double-click.
- Removed code-behind event handlers.

## [ui_agent] Add window Loaded and Exit commands
- Introduced LoadedCommand and ExitCommand in MainViewModel.
- Bound LoadedCommand via attached behavior and Escape key via InputBinding in MainWindow.
- Removed MainWindow code-behind logic.

## [ui_agent] Move InvoiceListView loaded logic to ViewModel
- Added LoadedCommand in InvoiceListViewModel to select the first invoice and set focus via INavigationService.
- Bound InvoiceListView's Loaded event using LoadedBehavior.
- Removed UserControl_Loaded method from code-behind.

## [ui_agent] Add AddNewCommand for editable combos
- Introduced AddNewCommand in EditableComboWithAddViewModel to directly open the entity creation dialog.
- Bound the Add button in EditableComboWithAdd.xaml to this command.
## [ui_agent] Introduce invoice screen states
- Added InvoiceScreenState enum and ScreenState property in MainViewModel.
- Updated commands and methods to switch states and control detail visibility.
## [startup_agent] Inject supplier dependencies in invoice detail
- Updated InvoiceDetailViewModel registration to supply supplier service and dialog.


## [doc_agent] Document supplier selector and screen states
- Added usage notes for the supplier selector component.
- Described browsing and editing screen states in docs/UX.md.

## [ui_agent] Fix missing Linq using
- Added System.Linq import in MainViewModel for FirstOrDefault.

## [ui_agent] Preselect payment method on new invoice
- Set PaymentMethodSelector.SelectedItem to the first item after creating the invoice object in MainViewModel.NewInvoice.
- Ensures default selection similar to SupplierSelector.

## [ui_agent] Async view model loading
- Replaced synchronous initialization with InitializeAsync in InvoiceItemInputViewModel, NewProductDialogViewModel, and InvoiceDetailViewModel.
- Added LoadCommand properties and bound them to Loaded events in corresponding views.
- Updated NewProductDialogService to stop calling Initialize().
## [ui_agent] Filter combo box items based on input
- Added FilteredItems ICollectionView in EditableComboWithAddViewModel to filter Items as Input changes.
- Updated EditableComboWithAdd view to bind ComboBox.ItemsSource to FilteredItems.

## [ui_agent] Add IDataErrorInfo to unit and supplier dialogs
Implemented property-level validation in NewSupplierDialogViewModel and
NewUnitDialogViewModel with duplicate checks. Updated associated XAML to
use validation templates from LightTheme.
## [test_agent] Validate dialog view model errors
Added unit tests covering required and duplicate name rules for the new
IDataErrorInfo implementations.
## [ui_agent] Add status bar view
- Introduced StatusBarViewModel with hint text properties.
- Created StatusBarView bound to those hints and placed it in MainWindow.
## [ui_agent] Fix behaviors namespace for XAML build errors
Replaced `http://schemas.microsoft.com/xaml/behaviors` with Expression Blend
namespaces in views and switched actions to the `ei` prefix to resolve
`Interaction.Triggers` compile errors.
