# API Documentation

This document provides a detailed reference for the Forex Rate Alerter API.

## Base URL

The API is served from the root of the application URL.

-   Local Development: `http://localhost:5000/api` or `https://localhost:5001/api`

## Authentication

Most endpoints are protected and require a JSON Web Token (JWT) to be included in the `Authorization` header of the request.

-   **Header Format**: `Authorization: Bearer <YOUR_JWT_TOKEN>`

To obtain a token, you must first register a user and then log in.

---

## 1. Authentication (`/api/auth`)

Endpoints for user registration and login.

### `POST /api/auth/register`

Registers a new user in the system.

-   **Authentication**: None
-   **Request Body**:

    ```json
    {
      "email": "user@example.com",
      "password": "YourStrongPassword123!",
      "firstName": "John",
      "lastName": "Doe"
    }
    ```

-   **Success Response (200 OK)**:

    ```json
    {
      "message": "User registered successfully",
      "user": {
        "id": 1,
        "email": "user@example.com",
        "firstName": "John",
        "lastName": "Doe",
        "role": "User",
        "createdAt": "2025-08-21T10:00:00Z"
      }
    }
    ```

-   **Error Response (400 Bad Request)**:
    -   If the email already exists or validation fails.
    ```json
    {
      "error": "User with this email already exists"
    }
    ```

### `POST /api/auth/login`

Authenticates a user and returns a JWT.

-   **Authentication**: None
-   **Request Body**:

    ```json
    {
      "email": "user@example.com",
      "password": "YourStrongPassword123!"
    }
    ```

-   **Success Response (200 OK)**:

    ```json
    {
      "token": "ey...",
      "message": "Login successful"
    }
    ```

-   **Error Response (401 Unauthorized)**:
    -   If credentials are invalid.
    ```json
    {
      "error": "Invalid email or password"
    }
    ```

---

## 2. Alerts (`/api/alert`)

Endpoints for managing user-specific alerts.

### `POST /api/alert`

Creates a new alert for the authenticated user.

-   **Authentication**: Required (User role)
-   **Request Body**:

    ```json
    {
      "baseCurrency": "USD",
      "targetCurrency": "MWK",
      "condition": 1, // 1: GreaterThan, 2: LessThan, 3: EqualTo
      "targetRate": 1750.50
    }
    ```

-   **Success Response (201 Created)**:
    -   Returns the newly created alert.
    ```json
    {
      "id": 101,
      "baseCurrency": "USD",
      "targetCurrency": "MWK",
      "condition": "GreaterThan",
      "targetRate": 1750.50,
      "isActive": true,
      "createdAt": "2025-08-21T11:00:00Z",
      "lastTriggeredAt": null
    }
    ```

### `GET /api/alert`

Retrieves all alerts for the authenticated user.

-   **Authentication**: Required (User role)
-   **Success Response (200 OK)**:

    ```json
    {
      "alerts": [
        {
          "id": 101,
          "baseCurrency": "USD",
          "targetCurrency": "MWK",
          "condition": "GreaterThan",
          "targetRate": 1750.50,
          "isActive": true,
          "createdAt": "2025-08-21T11:00:00Z",
          "lastTriggeredAt": null
        }
      ]
    }
    ```

### `GET /api/alert/{id}`

Retrieves a specific alert by its ID.

-   **Authentication**: Required (User role)
-   **Success Response (200 OK)**:

    ```json
    {
      "id": 101,
      "baseCurrency": "USD",
      "targetCurrency": "MWK",
      "condition": "GreaterThan",
      "targetRate": 1750.50,
      "isActive": true,
      "createdAt": "2025-08-21T11:00:00Z",
      "lastTriggeredAt": null
    }
    ```
-   **Error Response (404 Not Found)**: If the alert does not exist or does not belong to the user.

### `PUT /api/alert/{id}`

Updates an existing alert.

-   **Authentication**: Required (User role)
-   **Request Body**:
    -   Provide only the fields you want to update.
    ```json
    {
      "targetRate": 1800.00,
      "isActive": false
    }
    ```

-   **Success Response (200 OK)**:
    -   Returns the updated alert.

### `DELETE /api/alert/{id}`

Deletes an alert.

-   **Authentication**: Required (User role)
-   **Success Response (204 No Content)**
-   **Error Response (404 Not Found)**: If the alert does not exist or does not belong to the user.

---

## 3. Exchange Rates (`/api/exchangerate`)

Endpoints for retrieving exchange rate data.

### `GET /api/exchangerate/latest`

Gets the latest stored rates for all supported currency pairs.

-   **Authentication**: Required (User role)
-   **Success Response (200 OK)**:

    ```json
    {
      "rates": [
        {
          "id": 1,
          "baseCurrency": "USD",
          "targetCurrency": "MWK",
          "rate": 1745.1234,
          "timestamp": "2025-08-21T12:00:00Z",
          "source": "ExchangeRate-API"
        }
      ],
      "timestamp": "2025-08-21T12:01:00Z"
    }
    ```

### `GET /api/exchangerate/latest/{baseCurrency}/{targetCurrency}`

Gets the latest rate for a specific currency pair.

-   **Authentication**: Required (User role)
-   **Example URL**: `/api/exchangerate/latest/USD/MWK`
-   **Success Response (200 OK)**:

    ```json
    {
      "id": 1,
      "baseCurrency": "USD",
      "targetCurrency": "MWK",
      "rate": 1745.1234,
      "timestamp": "2025-08-21T12:00:00Z",
      "source": "ExchangeRate-API"
    }
    ```

### `GET /api/exchangerate/history/{baseCurrency}/{targetCurrency}`

Gets the historical rate data for a pair.

-   **Authentication**: Required (User role)
-   **Query Parameters**:
    -   `days` (integer, optional, default: 30): The number of past days of history to retrieve.
-   **Example URL**: `/api/exchangerate/history/USD/MWK?days=7`
-   **Success Response (200 OK)**:

    ```json
    {
      "history": [
        {
          "id": 1,
          "baseCurrency": "USD",
          "targetCurrency": "MWK",
          "rate": 1745.1234,
          "timestamp": "2025-08-21T12:00:00Z",
          "source": "ExchangeRate-API"
        }
      ],
      "days": 7
    }
    ```

### `POST /api/exchangerate/refresh`

Manually triggers the service to fetch the latest rates from the external API.

-   **Authentication**: Required (**Admin role only**)
-   **Success Response (200 OK)**:

    ```json
    {
      "message": "Exchange rates updated successfully"
    }
    ```

---

## 4. Admin (`/api/admin`)

Endpoints for system administration. All endpoints here require Admin role.

### `GET /api/admin/users`

Retrieves a list of all users in the system.

-   **Authentication**: Required (Admin role)
-   **Success Response (200 OK)**:

    ```json
    {
      "users": [
        {
          "id": 1,
          "email": "user@example.com",
          "firstName": "John",
          "lastName": "Doe",
          "role": "User",
          "isActive": true,
          "createdAt": "2025-08-21T10:00:00Z",
          "alertCount": 5
        }
      ]
    }
    ```

### `GET /api/admin/alerts`

Retrieves all active alerts currently in the system.

-   **Authentication**: Required (Admin role)
-   **Success Response (200 OK)**:

    ```json
    {
      "alerts": [
        {
          "id": 101,
          "baseCurrency": "USD",
          "targetCurrency": "MWK",
          "condition": "GreaterThan",
          "targetRate": 1750.50,
          "createdAt": "2025-08-21T11:00:00Z",
          "lastTriggeredAt": null,
          "userEmail": "user@example.com"
        }
      ]
    }
    ```

### `GET /api/admin/alert-logs`

Retrieves a paginated list of all alert notification logs.

-   **Authentication**: Required (Admin role)
-   **Query Parameters**:
    -   `page` (integer, optional, default: 1)
    -   `pageSize` (integer, optional, default: 50)
-   **Success Response (200 OK)**:

    ```json
    {
      "logs": [
        {
          "id": 1,
          "alertId": 101,
          "currencyPair": "USD/MWK",
          "triggeredRate": 1751.00,
          "targetRate": 1750.50,
          "condition": "GreaterThan",
          "triggeredAt": "2025-08-21T12:00:00Z",
          "emailSent": true,
          "emailError": null,
          "userEmail": "user@example.com"
        }
      ],
      "pagination": {
        "page": 1,
        "pageSize": 50,
        "totalCount": 1,
        "totalPages": 1
      }
    }
    ```

### `GET /api/admin/statistics`

Retrieves key statistics about the system.

-   **Authentication**: Required (Admin role)
-   **Success Response (200 OK)**:

    ```json
    {
      "totalUsers": 50,
      "totalAlerts": 200,
      "alertsTriggeredToday": 15,
      "alertsTriggeredThisWeek": 120,
      "mostPopularCurrencyPairs": [
        {
          "currencyPair": "USD/MWK",
          "count": 45
        },
        {
          "currencyPair": "EUR/USD",
          "count": 30
        }
      ]
    }
    ```
