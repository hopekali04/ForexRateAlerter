# Forex Rate Alerter

## Overview

The **Forex Rate Alerter** is a robust, enterprise-grade .NET application designed to monitor foreign exchange (Forex) rates and provide real-time alerts to users. Built with a clean, scalable architecture, this system is ideal for financial institutions, traders, and businesses that require timely and accurate Forex information.

Users can register, define custom alerts for specific currency pairs (e.g., notify me when USD/MWK is greater than 1750.00), and receive instant email notifications when their predefined conditions are met. The application also features a secure administrative dashboard for monitoring system activity and user management.

This project is built with security, reliability, and maintainability as top priorities, making it a perfect example of a production-ready financial technology application.

## Key Features

-   **User Authentication**: Secure user registration and JWT-based authentication.
-   **Custom Alerts**: Create, manage, and customize alerts based on currency pairs and rate conditions (greater than, less than, equal to).
-   **Real-time Rate Monitoring**: The system periodically fetches and stores the latest Forex rates from a reliable external API.
-   **Instant Email Notifications**: A background service continuously checks for triggered alerts and sends immediate email notifications.
-   **Administrative Dashboard**: A secure area for administrators to view all users, active alerts, system statistics, and notification logs.
-   **Historical Data**: View historical rate data for currency pairs.
-   **Clean Architecture**: A well-organized, multi-layered solution promoting separation of concerns and testability.

## Documentation Suite

This project includes a comprehensive documentation suite to provide a deep understanding of its design, setup, and usage.

-   **[Architecture Overview](./ARCHITECTURE.md)**: A detailed explanation of the system's architecture, design patterns, database schema, and security considerations.
-   **[API Documentation](./API.md)**: A complete reference for all API endpoints, including request/response examples and authentication requirements.
-   **[Setup Guide](./SETUP.md)**: Step-by-step instructions for setting up and running the project in a local development environment.

## Technologies Used

-   **.NET 7**: Core framework for the application.
-   **ASP.NET Core**: For building the robust REST API.
-   **Entity Framework Core**: For data access and database management.
-   **SQL Server**: As the primary relational database.
-   **JWT (JSON Web Tokens)**: For securing API endpoints.
-   **Serilog**: For structured and flexible logging.
-   **Swagger/OpenAPI**: For API documentation and testing.
-   **xUnit & Moq**: For unit and integration testing.