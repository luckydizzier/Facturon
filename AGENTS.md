# AGENTS.md – Development Strategy for Codex-Assisted Agents

This project uses an agent-based development workflow to structure Codex tasks, ensure modularity, and enforce discipline during development.

## Agent Roles

| Agent Name | Role | Folder Scope | Primary Tasks |
|------------|------|--------------|----------------|
| `[domain_agent]` | Domain modeling expert | `Domain/Entities` | Define core data entities, their relations, constraints |
| `[db_agent]` | Persistence & EF Core agent | `Data/`, `Migrations/` | Configure DbContext, migrations, connection handling |
| `[service_agent]` | Business logic & validation | `Services/` | Implement domain rules and application logic |
| `[repo_agent]` | Repository architect | `Repositories/` | Create interfaces and concrete EF Core repositories |
| `[ui_agent]` | MVVM & WPF UI designer | `ViewModels/`, `Views/`, `Themes/` | Create keyboard-only UI and ViewModel bindings |
| `[startup_agent]` | App orchestration & DI | `Startup/` | Configure DI, logging, database init |
| `[test_agent]` | Testing & validation | `Tests/` | Create unit/integration tests |
| `[doc_agent]` | Documentation coordinator | Root | Maintain all *.md docs, diagrams |

## Codex Prompt Example

[domain_agent]
Create Product.cs in Domain/Entities:

Fields: Id, Name, UnitId, ProductGroupId, TaxRateId
All navigation properties should be virtual
Use required attributes where appropriate


## Agent Protocol

- Each agent must generate only within its folder scope.
- Any cross-cutting changes must be coordinated by `[orchestrator_agent]`.
- All prompts must be documented in `PROMPT_LOG.md`.

## Guidelines

- Consistency is more important than cleverness
- Small, auditable changes > big, risky ones
- Always document agent assumptions
