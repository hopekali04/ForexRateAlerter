# Project Setup Guide

This guide provides detailed, step-by-step instructions for setting up the Forex Rate Alerter application in a local development environment.

## Prerequisites

Before you begin, ensure you have the following tools installed on your machine:

-   **.NET 7 SDK**: [Download & Install .NET 7](https://dotnet.microsoft.com/download/dotnet/7.0)
-   **SQL Server**: A local instance of SQL Server (e.g., SQL Server Express or SQL Server Developer Edition).
-   **Git**: For cloning the repository.
-   **An IDE or Code Editor**:
    -   Visual Studio 2022
    -   JetBrains Rider
    -   Visual Studio Code with C# extensions

## 1. Clone the Repository

First, clone the project repository to your local machine using Git.

```bash
git clone <your-repository-url>
cd ForexRateAlerter
```

## 2. Configure Application Secrets

This application uses `appsettings.json` and user secrets to manage configuration, ensuring that sensitive data like connection strings and API keys are not committed to source control.

#### a. Set up the Database Connection

The primary database connection string is required. You can set this up using the .NET user secrets manager.

1.  Navigate to the API project directory:
    ```bash
    cd src/ForexRateAlerter.Api
    ```

2.  Initialize user secrets for the project:
    ```bash
    dotnet user-secrets init
    ```

3.  Set the database connection string. **Remember to replace the placeholders** with your actual SQL Server details.

    ```bash
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER_NAME;Database=ForexRateAlerterDB;User Id=YOUR_USER_ID;Password=YOUR_PASSWORD;Trusted_Connection=False;Encrypt=True;TrustServerCertificate=True;"
    ```
    *Note: `TrustServerCertificate=True` is often needed for local development without a trusted SSL certificate. For production, proper certificate validation is recommended.*

#### b. Configure JWT Settings

Set the JWT secrets for token generation.

```bash
dotnet user-secrets set "Jwt:Key" "A_VERY_SECURE_AND_LONG_SECRET_KEY_FOR_JWT"
dotnet user-secrets set "Jwt:Issuer" "https://forexratealerter.com"
dotnet user-secrets set "Jwt:Audience" "https://forexratealerter.com"
```

#### c. Configure Email (SMTP) Settings

The application sends email notifications via SMTP. You can use a service like SendGrid, Mailgun, or a local SMTP server (like `smtp4dev` for testing).

```bash
dotnet user-secrets set "Email:SmtpHost" "your_smtp_host"
dotnet user-secrets set "Email:SmtpPort" "587"
dotnet user-secrets set "Email:SmtpUser" "your_smtp_username"
dotnet user-secrets set "Email:SmtpPassword" "your_smtp_password"
dotnet user-secrets set "Email:FromEmail" "noreply@forexratealerter.com"
```

#### d. Configure the Exchange Rate API Key

The application requires an API key from a provider to fetch exchange rates.

```bash
dotnet user-secrets set "ExchangeRateApi:ApiKey" "YOUR_EXCHANGE_RATE_API_KEY"
dotnet user-secrets set "ExchangeRateApi:BaseUrl" "https://api.exchangerate-api.com/v4" # Or your provider's URL
```

## 3. Apply Database Migrations

Once the connection string is configured, you need to create the database and apply the schema using Entity Framework Core migrations.

1.  Ensure you are in the `src/ForexRateAlerter.Api` directory.
2.  Run the following command to apply the migrations:

    ```bash
    dotnet ef database update
    ```
    This command will connect to your SQL Server instance, create the `ForexRateAlerterDB` database (if it doesn't exist), and create all the necessary tables.

## 4. Build and Run the Application

Now you are ready to build and run the application.

1.  Navigate back to the solution root directory (or stay in the API directory).
2.  Run the application using the .NET CLI:

    ```bash
    cd ../../ # Navigate back to the root
    dotnet run --project src/ForexRateAlerter.Api/ForexRateAlerter.Api.csproj
    ```

3.  The API will start, and you should see output in your console indicating that it is listening on a specific port (e.g., `http://localhost:5000` or `https://localhost:5001`).

## 5. Access the API Documentation

Once the application is running, you can access the Swagger UI in your web browser to explore and interact with the API endpoints.

-   Navigate to: **`http://localhost:<port>`** (e.g., `http://localhost:5000`)

You will see the Swagger documentation, where you can test the endpoints, including user registration, login, and creating alerts.

---

You have now successfully set up and launched the Forex Rate Alerter application.
