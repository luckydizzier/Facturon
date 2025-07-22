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
## [ui_agent] Add debug logging and no-data view
## [ui_agent] Add missing view constructors
Added constructors in InvoiceListView and InvoiceDetailView to call InitializeComponent so their XAML content renders.
