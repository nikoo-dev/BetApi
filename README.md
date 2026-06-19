# BetApi

![Build & Test](https://github.com/nikoo-dev/BetApi/actions/workflows/build.yml/badge.svg)

A RESTful Sports Betting API built with .NET 8, designed as a portfolio project targeting backend developer roles in the online gaming industry. Includes a lightweight HTML/CSS/JS frontend client.

## Tech Stack

**Backend**
- **ASP.NET Core 8** — Web API framework
- **Entity Framework Core** — ORM with Code First migrations
- **PostgreSQL** — relational database
- **JWT + Refresh Tokens** — full OAuth 2 token flow
- **BCrypt** — password hashing
- **xUnit + Moq** — unit testing
- **Docker** — containerized deployment
- **Swagger** — interactive API documentation

**Frontend**
- **HTML / CSS / JavaScript** — no framework, vanilla client
- Token-based auth with automatic refresh
- Live odds selection and bet placement

## Features

- User registration and login with JWT + refresh token rotation
- Role-based access control (Player / Admin)
- Browse available sports games with live odds
- Place bets with automatic odds locking at time of placement
- Automatic bet settlement when admin sets game result
- Wallet system with full transaction history
- Global error handling with clean JSON responses
- Auto-migration and database seeding on startup
- Working frontend client to register, log in, browse games, place bets, and track bet history

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
cd BetApi/BetliveApi
docker-compose up
```

API runs at `http://localhost:8080` · Swagger at `http://localhost:8080`

### Option 2: Local development

**Prerequisites:** .NET 8 SDK · PostgreSQL

```bash
# 1. Clone the repo
git clone https://github.com/nikoo-dev/BetApi.git
cd BetApi/BetliveApi

# 2. Update connection string in appsettings.json
# "DefaultConnection": "Host=localhost;Database=betlive_db;Username=postgres;Password=YOUR_PASSWORD"

# 3. Restore packages
dotnet restore

# 4. Apply migrations
dotnet ef database update

# 5. Run — seeding happens automatically on first run
dotnet run
```

### Default Admin Account (seeded automatically on first run)
```
Email:    admin@betlive.ge
Password: Admin123!
```

### Running the Frontend

1. Make sure the API is running (see above) and note the port it's listening on
2. Open `Frontend/app.js` and confirm `API_BASE` matches your API's URL and port
3. Open `Frontend/index.html` with a local server (e.g. VS Code's **Live Server** extension)
4. Register a new account or log in, then browse games and place bets

## Running Tests

```bash
dotnet test
```

15 unit tests covering `BetService` and `AuthService` business logic using xUnit + Moq.

| Test class | Tests |
|---|---|
| `BetServiceTests` | PlaceBet (valid, insufficient balance, game not upcoming, user not found, balance deduction, odds locking), SettleGameBets (winner credited, loser marked) |
| `AuthServiceTests` | Register (success, duplicate email, duplicate username), Login (valid, wrong password, nonexistent email) |

## Project Structure

```
BetApi/
├── BetliveApi/
│   ├── Controllers/          # HTTP endpoints
│   ├── Services/             # Business logic
│   │   └── Interfaces/
│   ├── Repositories/         # Data access layer
│   │   └── Interfaces/
│   ├── Models/               # EF Core entities
│   ├── DTOs/                 # Request / response shapes
│   │   ├── Auth/
│   │   ├── Bet/
│   │   ├── Game/
│   │   └── User/
│   ├── Data/                 # DbContext + seeder
│   ├── Middleware/            # Global error handling
│   ├── Extensions/            # DI + validation + CORS configuration
│   ├── Migrations/           # EF Core migrations
│   ├── Frontend/             # HTML/CSS/JS client
│   ├── Dockerfile
│   └── docker-compose.yml
└── BetliveApi.Tests/
    └── Services/             # Unit tests
```

## Status

✅ Complete — full-stack demo with working backend, frontend, tests, and CI/CD
