# Forex Rate Alerter

## Overview

The **Forex Rate Alerter** is a robust, enterprise-grade .NET application designed to monitor foreign exchange (Forex) rates and provide real-time alerts to users. Built with a clean, scalable architecture, this system is ideal for financial institutions, traders, and businesses that require timely and accurate Forex information.

Users can register, define custom alerts for specific currency pairs (e.g., notify me when USD/MWK is greater than 1750.00), and receive instant email notifications when their predefined conditions are met. The application also features a secure administrative dashboard for monitoring system activity and user management.

This project is built with security, reliability, and maintainability as top priorities, making it a perfect example of a production-ready financial technology application. The system features a sophisticated dual-API syncing architecture with background services that continuously maintain both baseline market data and calculated synthetic rates.

## Key Features

-   **User Authentication**: Secure user registration and JWT-based authentication.
-   **Custom Alerts**: Create, manage, and customize alerts based on currency pairs and rate conditions (greater than, less than, equal to).
-   **Dual-Source Rate Syncing**: Two complementary background services fetch data from different external APIs at different intervals for comprehensive rate coverage.
-   **Hourly Synthetic Rate Calculation**: Advanced triangular arbitrage algorithm calculates cross-rates for all supported currency pairs every hour.
-   **Daily Baseline Refresh**: Every 24 hours, a baseline data fetch from an independent API ensures market accuracy.
-   **Instant Email Notifications**: A background alert-checking service runs hourly to detect triggered alerts and send immediate email notifications.
-   **Administrative Dashboard**: An area for admins to view all users, active alerts, system statistics, and notification logs.
-   **Historical Data & OHLC Analytics**: Full historical rate tracking with Open-High-Low-Close (OHLC) data and top movers analysis.
-   **Clean Architecture**: A well-organized, multi-layered solution promoting separation of concerns and testability.

## Background Service Architecture

The heart of the Forex Rate Alerter is its sophisticated background service ecosystem. Three background services work in harmony to maintain currency data freshness and process user alerts:

### 1. **Synthetic Exchange Rate Engine** (Hourly)
**Service**: `ExchangeRateSyncService`  
**Interval**: Every 1 hour (`resolution=1h`)  
**API Source**: FxRates API (`fxratesapi.com`)

The synthetic rate engine fetches base USD rates and calculates cross-rates for all supported currency pairs using a triangular arbitrage algorithm. This ensures comprehensive coverage of all currency pair combinations.

**How It Works**:
- Fetches the latest USD base rates from the FxRates API (with hourly resolution)
- Applies the formula: **Rate(A→B) = Rate(USD→B) / Rate(USD→A)**
- Generates rates for every permutation of supported currencies (O(N²) complexity)
- Implements change detection to only update rates when prices actually move
- Maintains full historical snapshots for analysis

**Key Technical Details**:
- Uses a USD base peg as the standard interbank reference
- Maintains precision at 6 decimal places (per API specification: `places=6`)
- Implements safety checks for divide-by-zero conditions
- Automatically performs database upserts for new/existing rate pairs

Example: To calculate **EUR/MWK**, the system uses:
```
EUR/MWK = (USD → MWK) / (USD → EUR)
```

### 2. **External Rate Fetch Service** (Daily)
**Service**: `ExternalRateFetchService`  
**Interval**: Every 24 hours  
**API Source**: ExchangeRate-API (`exchangerate-api.com`)

This service provides a daily baseline refresh from an independent external API, ensuring market accuracy and serving as a validation point for the synthetic calculations.

**How It Works**:
- Iterates through each supported base currency
- Fetches conversion rates for all target currencies
- Implements change detection to only store meaningful rate changes
- Maintains historical records for each rate change
- Logs comprehensive metrics (rate count, API health, timestamp)

**Key Technical Details**:
- Independent API source provides market baseline validation
- Runs with a 5-minute initial delay to avoid startup collisions
- Implements retry and error logging for reliability

### 3. **Alert Background Service** (Hourly)
**Service**: `AlertBackgroundService`  
**Interval**: Every 1 hour  
**Purpose**: Alert Detection & Notification

This service continuously monitors user-defined alert conditions and triggers email notifications when thresholds are met.

**How It Works**:
- Checks all active alerts against current rates
- Evaluates alert conditions (greater than, less than, equal to)
- Sends email notifications when conditions are triggered
- Logs all alert activity for audit trails
- Waits 30 seconds on startup to allow initial rate data to populate

## Supported Currencies

The system processes rates for: **USD, AUD, CAD, EUR, GBP, JPY, MWK, ZAR**

At full capacity with 8 currencies, the synthetic engine generates **56 unique currency pairs** (8 × 7) every hour, providing comprehensive coverage for forex trading and monitoring.

## Data Precision & Reliability

Following FinTech standards:
- **Decimal Precision**: All monetary values use `decimal(19, 4)` in the database (C# `decimal` type)
- **UTC Timezone**: All timestamps stored in UTC; frontend converts to local time at render layer
- **Change Detection**: Only rate changes exceeding 0.000001 precision trigger historical snapshots
- **Heartbeat Updates**: Even unchanged rates receive timestamp updates for freshness tracking

## Documentation Suite

This project includes a comprehensive documentation suite to provide a deep understanding of its design, setup, and usage.

-   **[Architecture Overview](./ARCHITECTURE.md)**: A detailed explanation of the system's architecture, design patterns, database schema, and security considerations.
-   **[API Documentation](./API.md)**: A complete reference for all API endpoints, including request/response examples and authentication requirements.
-   **[Setup Guide](./SETUP.md)**: Step-by-step instructions for setting up and running the project in a local development environment.
-   **[Currency Top Movers](./CURRENCY_TOP_MOVERS.md)**: How the historical trend tracking and top movers feature works.

## Technologies Used

### Backend
-   **.NET 8**: Core framework for the application.
-   **ASP.NET Core**: For building the robust REST API.
-   **Entity Framework Core**: For data access and database management with SQL Server.
-   **SQL Server**: As the primary relational database for persistent storage.
-   **JWT (JSON Web Tokens)**: For secure API endpoint authentication.
-   **Serilog**: For structured and flexible logging to console and files.
-   **Swagger/OpenAPI**: For interactive API documentation and testing.
-   **Polly**: For resilience patterns (retry, circuit breaker policies).
-   **NUnit & Moq**: For unit and integration testing.

### External Data Sources
-   **FxRates API** (`fxratesapi.com`): Primary source for hourly base USD rates with 1-hour resolution. Used by `ExchangeRateSyncService`.
-   **ExchangeRate-API** (`exchangerate-api.com`): Secondary source for daily baseline market data refresh. Used by `ExternalRateFetchService`.

### Frontend
-   **Vue.js**: Progressive JavaScript framework for the user interface.
-   **TypeScript**: Strongly typed JavaScript for improved maintainability.
-   **Vite**: Fast build tool and development server.
-   **Tailwind CSS**: Utility-first CSS framework for responsive design.
-   **ESLint & PostCSS**: For code quality and CSS processing.

## System Architecture at a Glance

```
┌─────────────────────────────────────────────────────────────────┐
│                     Forex Rate Alerter System                   │
└─────────────────────────────────────────────────────────────────┘

   BACKGROUND SERVICES (Always Running)
   ════════════════════════════════════════════════════════════
   
   Every 1 Hour ──────→ Synthetic Rate Engine
   (ExchangeRateSyncService)
   │
   ├─→ Fetch base USD rates from FxRates API
   ├─→ Calculate N² currency pairs via triangular arbitrage
   ├─→ Update rates & historical snapshots in database
   └─→ Change detection to avoid redundant writes
   
   Every 24 Hours ────→ External Rate Fetch Service
   (ExternalRateFetchService)
   │
   ├─→ Fetch from ExchangeRate-API (market baseline)
   ├─→ Validate data against synthetic calculations
   └─→ Store historical records
   
   Every 1 Hour ──────→ Alert Background Service
   (AlertBackgroundService)
   │
   ├─→ Evaluate user-defined alert conditions
   ├─→ Trigger email notifications
   └─→ Log alert activity

   DATA FLOW
   ════════════════════════════════════════════════════════════
   
   FxRates API ──┐
                 ├──→ ExchangeRateSyncService ──→ ExchangeRates Table
                 │                               ↓
                 │                          ExchangeRateHistory
   ExchangeRateAPI ──→ ExternalRateFetchService ──→
   
   Current Rates ──→ AlertBackgroundService ──→ Email Notifications
                     ↓
                  AlertLogs Table (Audit Trail)

   API & DATABASE
   ════════════════════════════════════════════════════════════
   
   REST API (ASP.NET Core) ←→ SQL Server Database
   ├─── /auth/** ─────────────── User Credentials
   ├─── /alerts/** ──────────── User Alerts & Logs
   ├─── /rates/** ──────────── Exchange Rate Data
   └─── /admin/** ──────────── Admin Dashboard
```