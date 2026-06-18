# BetApi

![Build & Test](https://github.com/nikoo-dev/BetApi/actions/workflows/build.yml/badge.svg)

A RESTful Sports Betting API built with .NET 8, designed as a portfolio project targeting backend developer roles in the online gaming industry.

## Tech Stack

- **ASP.NET Core 8** — Web API framework
- **Entity Framework Core** — ORM with Code First migrations
- **PostgreSQL** — relational database
- **JWT + Refresh Tokens** — full OAuth 2 token flow
- **BCrypt** — password hashing
- **xUnit + Moq** — unit testing
- **Docker** — containerized deployment
- **Swagger** — interactive API documentation

## Features

- User registration and login with JWT + refresh token rotation
- Role-based access control (Player / Admin)
- Browse available sports games with live odds
- Place bets with automatic odds locking at time of placement
- Automatic bet settlement when admin sets game result
- Wallet system with full transaction history
- Global error handling with clean JSON responses
- Auto-migration and database seeding on startup

## API Endpoints

### Auth — no token required
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register a new player account |
| POST | `/api/auth/login` | Login and receive JWT + refresh token |
| POST | `/api/auth/refresh` | Get a new JWT using a refresh token |
| POST | `/api/auth/revoke` | Logout / revoke refresh token |

### Games — reading is public, writing is Admin only
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/games` | Get all games |
| GET | `/api/games/upcoming` | Get upcoming games (bets open) |
| GET | `/api/games/{id}` | Get a single game |
| POST | `/api/games` | Create a game *(Admin)* |
| PATCH | `/api/games/{id}/result` | Set result and settle bets *(Admin)* |

### Bets — requires login
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/bets` | Place a bet on an upcoming game |
| GET | `/api/bets/my` | Get all your bets |

### Users — requires login
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users/me` | Get your profile |
| GET | `/api/users/me/balance` | Get your wallet balance |
| POST | `/api/users/me/deposit` | Deposit funds |
| GET | `/api/users/{id}` | Get any user *(Admin)* |

## Getting Started

### Option 1: Docker (recommended — no setup needed)

```bash
git clone https://github.com/nikoo-dev/BetApi.git
cd BetApi
docker-compose up
```

API runs at `http://localhost:8080` · Swagger at `http://localhost:8080/swagger`

### Option 2: Local development

**Prerequisites:** .NET 8 SDK · PostgreSQL

```bash
# 1. Clone the repo
git clone https://github.com/nikoo-dev/BetApi.git
cd BetApi

# 2. Update connection string in appsettings.json
# "DefaultConnection": "Host=localhost;Database=betlive_db;Username=postgres;Password=YOUR_PASSWORD"

# 3. Restore packages
dotnet restore

# 4. Run — migrations and seeding happen automatically
dotnet run
```

### Default Admin Account (seeded automatically)
```
Email:    admin@betlive.ge
Password: Admin123!
```

## Running Tests

```bash
dotnet test
```

13 unit tests covering `BetService` and `AuthService` business logic using xUnit + Moq.

## Project Structure

```
BetApi/
├── Controllers/          # HTTP endpoints
├── Services/             # Business logic
│   └── Interfaces/
├── Repositories/         # Data access layer
│   └── Interfaces/
├── Models/               # EF Core entities
├── DTOs/                 # Request / response shapes
│   ├── Auth/
│   ├── Bet/
│   ├── Game/
│   └── User/
├── Data/                 # DbContext + seeder
├── Middleware/           # Global error handling
├── Extensions/           # DI + validation configuration
├── Dockerfile
├── docker-compose.yml
└── .github/workflows/    # CI pipeline
```

## Status

✅ Complete — production-ready structure
