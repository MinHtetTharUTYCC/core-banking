# Core Banking System - Project Goals

## Overview
A production-grade core banking system built with C#/.NET to master advanced enterprise patterns including CQRS, MediatR, Domain-Driven Design, and Event Sourcing.

---

## Learning Objectives

### 1. CQRS (Command Query Responsibility Segregation)
- Separate read and write operations
- Different models for commands vs queries
- Optimize each side independently

### 2. MediatR Pipeline
- Request/Handler pattern (In-Memory Mediator)
- Behaviors (validation, logging, transaction)
- Decouple controllers from business logic

### 3. Domain-Driven Design (DDD)
- Aggregates: Account as aggregate root
- Value Objects: Money, AccountNumber
- Domain Events: AccountCreated, MoneyDeposited
- Bounded Contexts: Accounts, Transactions, Loans

### 4. Event Sourcing (Future)
- Store state changes as events
- Rebuild state from event history
- Complete audit trail

### 5. Entity Framework Core
- Code-First with migrations
- Complex relationships and value conversions
- Query filters (soft delete)
- Concurrency handling

---

## Modules to Build

### Phase 1: Account Management
- [x] Create account (savings, current, fixed deposit)
- [x] Account activation workflow
- [x] Account status management (pending, active, suspended, closed)
- [x] Balance tracking with Money value object

### Phase 2: Transaction Processing
- [x] Deposit money
- [x] Withdraw money
- [x] Internal transfers
- [x] Transaction history with pagination
- [x] Idempotency keys (prevent duplicates)
- [x] Transaction states (pending → processing → completed/failed)

### Phase 3: Interest Engine
- [ ] Different rates per account type
- [ ] Daily accrual calculation
- [ ] Monthly compounding
- [ ] Interest events (InterestAccruedEvent)

### Phase 4: Loan Management
- [ ] Loan application workflow
- [ ] Approval process
- [ ] Disbursement
- [ ] EMI tracking
- [ ] NPA classification

### Phase 5: Compliance & Audit
- [ ] Transaction limits
- [ ] AML checks
- [ ] Complete audit trail
- [ ] Regulatory reporting

---

## API Endpoints

### Accounts
```
POST   /api/accounts              - Create new account
GET    /api/accounts              - List all accounts
GET    /api/accounts/{id}         - Get account by ID
POST   /api/accounts/{id}/deposit    - Deposit money
POST   /api/accounts/{id}/withdraw   - Withdraw money
POST   /api/accounts/transfer        - Transfer between accounts
```

### Transactions (Future)
```
GET    /api/transactions              - List all transactions
GET    /api/transactions/{id}         - Get transaction by ID
GET    /api/accounts/{id}/transactions - Get account transactions
```

---

## Tech Stack

| Layer          | Technology                    |
|----------------|-------------------------------|
| API            | ASP.NET Core Web API          |
| CQRS           | MediatR 14.x                  |
| Validation     | FluentValidation              |
| ORM            | Entity Framework Core         |
| Database       | MSSQL Developer Edition       |
| Documentation  | Swashbuckle (Swagger)         |
| Testing        | xUnit (planned)               |

---

## Architecture Rules

1. **Domain layer has ZERO dependencies** - no framework references
2. **Application layer** depends only on Domain
3. **Infrastructure layer** implements Application interfaces
4. **API layer** references Application and Infrastructure
5. **No business logic in controllers** - delegate to MediatR

---

## Patterns to Implement

- [x] Aggregate Root (Account)
- [x] Value Objects (Money, AccountNumber)
- [x] Domain Events
- [x] Repository Pattern
- [ ] Unit of Work
- [x] CQRS
- [x] MediatR Pipeline Behaviors
- [x] FluentValidation
- [ ] Soft Delete
- [ ] Optimistic Concurrency
- [ ] Event Sourcing (Phase 5)
- [ ] Saga Pattern (Phase 5)

---

## Commands Cheat Sheet

```bash
# Create migration
dotnet ef migrations add <MigrationName> --project src/CoreBanking.Infrastructure --startup-project src/CoreBanking.Api

# Update database
dotnet ef database update --project src/CoreBanking.Infrastructure --startup-project src/CoreBanking.Api

# Run API
dotnet run --project src/CoreBanking.Api

# Build solution
dotnet build
```

---

## Success Criteria

1. Clean separation between read/write operations
2. Domain logic independent of infrastructure
3. Complete audit trail for all transactions
4. Handles concurrent operations safely
5. Production-ready error handling
6. Swagger documentation for all endpoints
