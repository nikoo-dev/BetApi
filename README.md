# BetApi

A RESTful Sports Betting API built with .NET 8, designed as a portfolio project targeting backend developer roles in the online gaming industry.

## Tech Stack

- **ASP.NET Core 8** — Web API framework
- **Entity Framework Core** — ORM with Code First migrations
- **PostgreSQL** — relational database
- **JWT / OAuth 2** — authentication and authorization
- **Swagger** — API documentation

## Features

- User registration and login with JWT tokens
- Role-based access (Player / Admin)
- Browse available sports games with odds
- Place bets and track results
- Wallet system with transaction history

## Project Structure

BetApi/

├── Controllers/     # API endpoints

├── Services/        # Business logic

├── Repositories/    # Data access layer

├── Models/          # EF Core entities

├── DTOs/            # Request/response shapes

├── Data/            # DbContext

├── Middleware/      # Global error handling

└── Extensions/      # DI configuration

## Getting Started

```bash
# Install dependencies
dotnet restore

# Set your connection string in appsettings.json

# Run migrations
dotnet ef migrations add InitialCreate
dotnet ef database update

# Run the API
dotnet run
```

## Status

🚧 In active development
