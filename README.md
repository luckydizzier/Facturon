# Facturon

**Facturon** is a domain-driven, keyboard-first invoice management application designed with clarity, performance, and extensibility in mind. It uses a modular architecture with EF Core, WPF (MVVM), and a structured agent-based development workflow.

## Key Principles

- 🧠 Domain-first design (Invoices, Products, Suppliers, Tax logic)
- ⌨️ Keyboard-only UI (WPF, no mouse required)
- 🧱 Layered architecture: Model → Repo → Service → ViewModel → View
- 🧪 Agent-based, testable, and refactor-friendly development

## Technologies

- C#, .NET 8+
- WPF (MVVM pattern)
- Entity Framework Core
- Serilog
- SQLite (initial target)

## Development Goals

- Clean separation of concerns
- Domain-model validation in service layer
- Configurable logging and behavior
- Easily extendable via plugin-style architecture

## Getting Started

_TBD_: initial setup, architecture map, build instructions.

## Portable Usage

When run outside an installer, the application looks for `appsettings.json`,
`facturon.db`, and a `logs` folder next to the executable. Missing files are
automatically created on first launch, allowing the app to run from any
location without setup.
