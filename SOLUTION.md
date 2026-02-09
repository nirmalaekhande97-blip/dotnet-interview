# Solution Documentation

**Completion Date:** January 2024  
**Project:** TODO API Refactoring - .NET 8

---

## Problems Identified

### Critical Security Issues

#### 1. **SQL Injection Vulnerabilities (CRITICAL)**
The original code used string concatenation for SQL queries:
```csharp
// VULNERABLE CODE
command.CommandText = $"SELECT * FROM Todos WHERE Id = {id}";
```

This allows attackers to inject malicious SQL. For example:
- Input: `1 OR 1=1; DROP TABLE Todos--`
- Result: Database compromised or destroyed

**Fix:** Implemented parameterized queries throughout the application.

---

#### 2. **No Dependency Injection (HIGH)**
Controllers directly instantiated services using `new`:
```csharp
var todoService = new TodoService();
```

**Problems:**
- Impossible to unit test in isolation
- Tight coupling between components
- No lifecycle management
- Violates SOLID principles (Dependency Inversion)

**Fix:** Implemented constructor-based dependency injection with interfaces.

---

### Architectural Problems

#### 3. **Incorrect HTTP Methods (MEDIUM)**
All operations used POST instead of proper HTTP verbs:
```csharp
[HttpPost("getTodo")]     // Should be GET
[HttpPost("updateTodo")]  // Should be PUT
[HttpPost("deleteTodo")]  // Should be DELETE
```

**Impact:** Non-RESTful API, caching issues, poor developer experience.

**Fix:** Implemented proper REST conventions with GET, POST, PUT, DELETE.

---

#### 4. **No Data Validation (MEDIUM)**
No input validation allowed invalid data to reach the database:
- Null or empty titles accepted
- No length constraints
- No type checking

**Fix:** Implemented Data Annotations with comprehensive validation.

---

#### 5. **No Repository Pattern (MEDIUM)**
Data access logic was mixed with business logic in the service layer, violating Single Responsibility Principle.

**Fix:** Created repository layer with interface abstraction (ITodoRepository).

---

#### 6. **Domain Models Exposed in API (MEDIUM)**
Using domain models directly in controllers creates:
- Over-posting security risks
- Tight coupling between API and database
- Cannot evolve API independently

**Fix:** Created separate DTOs (CreateTodoDto, UpdateTodoDto, TodoDto).

---

### Code Quality Issues

#### 7. **Synchronous Operations (LOW-MEDIUM)**
All database operations blocked threads, limiting scalability.

**Fix:** Converted to async/await pattern throughout.

---

#### 8. **Hardcoded Configuration (LOW-MEDIUM)**
Connection string duplicated in multiple files.

**Fix:** Centralized configuration using Options pattern and appsettings.json.

---

#### 9. **Poor Error Handling (LOW)**
Generic exception catching exposed internal details to clients.

**Fix:** Implemented structured logging and proper HTTP error responses.

---

#### 10. **Magic Numbers (LOW)**
Column indices hardcoded as numbers: `reader.GetInt32(0)`

**Fix:** Used named column access: `reader.GetInt32(reader.GetOrdinal("Id"))`.

---

## Architectural Decisions

### 1. Layered Architecture (3-Tier)

Chosen a **clean layered architecture** for separation of concerns:

```
┌─────────────────────────────────────────┐
│      Presentation Layer                 │  Controllers (HTTP concerns)
│      TodosController.cs                 │
└──────────────┬──────────────────────────┘
               │ Uses DTOs
               ▼
┌─────────────────────────────────────────┐
│      Business Logic Layer               │  Services (business rules)
│      TodoService.cs                     │
└──────────────┬──────────────────────────┘
               │ Uses Domain Models
               ▼
┌─────────────────────────────────────────┐
│      Data Access Layer                  │  Repositories (database)
│      TodoRepository.cs                  │
└─────────────────────────────────────────┘
```

**Why?**
- **Separation of Concerns:** Each layer has single responsibility
- **Testability:** Can test layers independently with mocks
- **Maintainability:** Changes isolated to specific layers
- **Scalability:** Can replace individual layers
- **Industry Standard:** Widely understood pattern

**Alternatives Considered:**
- Vertical Slice Architecture - Rejected: Overkill for simple CRUD
- Clean/Onion Architecture - Rejected: Too complex for current scope

---

### 2. Repository Pattern

Abstracted data access behind `ITodoRepository` interface.

**Why?**
- Can switch from SQLite to SQL Server/PostgreSQL without changing business logic
- Easy to mock for testing
- Follows Single Responsibility Principle
- Reusable across services

**Why Not EF Core?**
- For simple CRUD with SQLite, ADO.NET is sufficient
- Less overhead and dependencies
- More control over SQL
- Can migrate to EF Core later if complexity increases

---

### 3. DTO Pattern

Created separate request/response objects:
- `CreateTodoDto` - Input for creating TODOs
- `UpdateTodoDto` - Input for updating TODOs
- `TodoDto` - Output for all responses
- `Todo` - Internal domain model (never exposed)

**Why?**
- **Security:** Prevents over-posting attacks (client can't set Id on create)
- **Versioning:** Can change DTOs without changing domain models
- **Validation:** Different validation rules per operation
- **Flexibility:** API contract independent of database schema

---

### 4. Dependency Injection

Used constructor-based DI with scoped lifetimes:
```csharp
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();
```

**Why Scoped?**
- Lives for duration of HTTP request
- Prevents memory leaks
- Allows transaction support in future
- Appropriate for database operations

---

### 5. Async/Await

All I/O operations made asynchronous.

**Benefits:**
- Better scalability (threads not blocked during I/O)
- Improved throughput under load
- ASP.NET Core best practice
- Better resource utilization

---

### 6. RESTful Design

Proper HTTP methods and status codes:

| Operation | Method | Endpoint | Success Code |
|-----------|--------|----------|--------------|
| Create | POST | /api/todos | 201 Created |
| Get All | GET | /api/todos | 200 OK |
| Get One | GET | /api/todos/{id} | 200 OK |
| Update | PUT | /api/todos/{id} | 200 OK |
| Delete | DELETE | /api/todos/{id} | 204 No Content |

**Why REST?**
- Industry standard
- Caching support (GET requests)
- Idempotency (PUT/DELETE)
- Tool support (Swagger, Postman)

---

## Trade-offs

### What I Prioritized

1. **Security First** - Eliminating SQL injection was top priority
2. **Testability** - Created mockable architecture with DI
3. **Clean Architecture** - Proper layer separation
4. **RESTful Design** - Following HTTP standards
5. **Input Validation** - Preventing bad data

### What I Deferred

1. **Entity Framework Core** - ADO.NET sufficient for now
2. **Integration Tests** - Focused on unit tests first
3. **Pagination** - Can add when needed
4. **Authentication** - Not in original requirements
5. **Caching** - Optimize later if needed
6. **Advanced Error Handling** - Basic logging sufficient for now

### Compromises Made

1. **Kept SQLite** - Instead of SQL Server (simplicity for demo)
2. **Basic Logging** - Instead of Application Insights (cost/complexity)
3. **Manual Mapping** - Instead of AutoMapper (one less dependency)
4. **Simple Validation** - Data Annotations instead of FluentValidation

### Alternatives Considered

1. **CQRS Pattern** - Rejected: Overkill for simple CRUD
2. **MediatR** - Rejected: Adds complexity without clear benefit
3. **Specification Pattern** - Rejected: Not needed for simple queries
4. **Unit of Work** - Rejected: Single repository is sufficient

---



