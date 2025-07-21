# Prompt Log

## [domain_agent] Initialize core domain model files for Facturon
Created initial domain entity classes under `Domain/Entities`.

## [db_agent] Added FacturonDbContext with Fluent API configurations

## [repo_agent] Implement EF Core repositories
Created base repository abstractions and EF Core implementations for Invoice and Product entities under `Repositories/`.
## [repo_agent] Extend repositories with querying and patch support
Added AsQueryable, GetByConditionAsync, PatchAsync to base repository and interfaces. Implemented repositories for Supplier, TaxRate, PaymentMethod, Unit, ProductGroup.
