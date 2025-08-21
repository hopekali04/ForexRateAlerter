# System Architecture

This document provides a comprehensive overview of the Forex Rate Alerter's technical architecture, design principles, and key components. It is intended for developers, architects, and technical stakeholders.

## 1. Architectural Approach: Clean Architecture

The application is built following the principles of **Clean Architecture**. This architectural style promotes a clear separation of concerns, resulting in a system that is highly maintainable, scalable, and testable.

The core idea is to create a dependency rule where inner layers (Core) are independent of outer layers (Infrastructure, API). This means that business logic and application models are not tied to any specific database, UI, or external service.

Our implementation is structured into four main projects:

-   **`ForexRateAlerter.Core`**: The heart of the application. It contains all the business models (entities), DTOs, and interfaces that define the core business logic. This project has **zero dependencies** on other projects in the solution.
-   **`ForexRateAlerter.Infrastructure`**: Implements the interfaces defined in the Core layer. It handles all external concerns, such as database access (via Entity Framework Core), email sending (SMTP), and communication with third-party APIs.
-   **`ForexRateAlerter.Api`**: The presentation layer. It exposes the application's functionality via a RESTful API built with ASP.NET Core. It depends on both the Core and Infrastructure layers to handle HTTP requests and orchestrate business logic.
-   **`ForexRateAlerter.Tests`**: Contains all unit and integration tests, ensuring the reliability and correctness of the application.

This layered approach ensures that a change in one part of the system (e.g., swapping the database from SQL Server to PostgreSQL) has a minimal impact on the rest of the codebase.

## 2. Database Structure

The database schema is designed to be simple, efficient, and relational. It is managed using Entity Framework Core code-first migrations.

### Key Entities

-   **`User`**: Stores user information, including credentials and role.
    -   `Id`, `Email`, `PasswordHash`, `FirstName`, `LastName`, `Role`, `IsActive`, `CreatedAt`
    -   The `Email` field is unique.
-   **`Alert`**: Represents a user-defined alert.
    -   `Id`, `UserId`, `BaseCurrency`, `TargetCurrency`, `Condition`, `TargetRate`, `IsActive`, `LastTriggeredAt`
    -   It has a many-to-one relationship with the `User` entity.
-   **`ExchangeRate`**: Stores historical exchange rate data fetched from the external API.
    -   `Id`, `BaseCurrency`, `TargetCurrency`, `Rate`, `Timestamp`, `Source`
    -   This table acts as a historical record and the source for the latest rates.
-   **`AlertLog`**: Records every instance an alert is triggered and a notification is sent.
    -   `Id`, `AlertId`, `TriggeredRate`, `TargetRate`, `Condition`, `TriggeredAt`, `EmailSent`, `EmailError`
    -   It has a many-to-one relationship with the `Alert` entity.

### Relationships

-   A `User` can have many `Alerts`.
-   An `Alert` can have many `AlertLogs`.

This normalized structure minimizes data redundancy and ensures data integrity through foreign key constraints.

## 3. Key Design Decisions & Patterns

-   **Dependency Injection (DI)**: DI is used extensively throughout the application, managed by the built-in ASP.NET Core IoC container. Services are registered with interfaces (e.g., `IAlertService`), allowing for loose coupling and easy testability.
-   **Service/Repository Pattern**: The `Infrastructure` layer implements interfaces from the `Core` layer, effectively acting as a repository and service layer. This abstracts data access and business logic from the API controllers.
-   **Asynchronous Programming (`async`/`await`)**: All I/O-bound operations (database calls, HTTP requests) are asynchronous. This ensures the application is non-blocking, efficient, and can handle a high number of concurrent requests.
-   **Background Services (`IHostedService`)**: The `AlertBackgroundService` runs as a long-running background task. It is responsible for periodically fetching new exchange rates and processing alerts, decoupling this critical task from the user-facing API.
-   **DTOs (Data Transfer Objects)**: DTOs are used to transfer data between the API and its clients. This prevents exposing the internal database models directly, providing an anti-corruption layer and allowing the API contract to evolve independently of the database schema.
-   **Configuration Management**: The application uses a combination of `appsettings.json` and the .NET Secret Manager. This allows for environment-specific configuration while keeping sensitive data (API keys, connection strings) secure and out of source control.

## 4. Testing Strategy

A comprehensive testing strategy is in place to ensure code quality and system reliability.

-   **Unit Tests (`ForexRateAlerter.Tests/Services`)**:
    -   These tests focus on individual components in isolation, primarily the services in the `Infrastructure` layer.
    -   Dependencies are mocked using the **Moq** framework.
    -   The in-memory database provider for Entity Framework Core is used to test data access logic without needing a real database.
-   **Integration Tests (`ForexRateAlerter.Tests/Integration`)**:
    -   These tests verify the interactions between different parts of the application, from the API controllers down to the database.
    -   They use the `WebApplicationFactory` to host the application in memory.
    -   HTTP requests are sent to the in-memory server, and responses are asserted, providing end-to-end testing of the API endpoints.

This two-tiered approach ensures that both individual logic units and the integrated system behave as expected.

## 5. Security Considerations

Security is a critical aspect of this application, especially given its potential use in a financial context.

-   **Authentication**:
    -   User authentication is handled via **JWT (JSON Web Tokens)**.
    -   Passwords are never stored in plain text. They are hashed using the robust **BCrypt** algorithm.
-   **Authorization**:
    -   Role-based authorization is implemented using ASP.NET Core Identity attributes (`[Authorize(Roles = "Admin")]`).
    -   This ensures that sensitive endpoints (like the admin dashboard) are only accessible to authorized personnel.
-   **Data Protection**:
    -   Sensitive configuration data is managed via the .NET Secret Manager, preventing secrets from being checked into Git.
    -   HTTPS should be enforced in a production environment to encrypt all data in transit.
-   **Input Validation**:
    -   All incoming DTOs are validated using Data Annotations (`[Required]`, `[MaxLength]`, etc.). This prevents invalid or malicious data from entering the system.
-   **Error Handling & Logging**:
    -   The application uses **Serilog** for structured logging.
    -   Exceptions are handled gracefully, and sensitive information is not leaked in error messages. Logs provide a crucial audit trail for security investigations.

## 6. Scalability & Performance

-   The use of asynchronous programming and a stateless API design allows the application to be scaled horizontally by deploying multiple instances behind a load balancer.
-   The background service offloads heavy processing (rate fetching, alert checking) from the main API threads, ensuring the API remains responsive to user requests.
-   Database indexing is used on key columns (e.g., user emails, currency pairs) to ensure fast query performance, as defined in the `ApplicationDbContext`.
