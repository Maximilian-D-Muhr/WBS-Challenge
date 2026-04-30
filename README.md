# ChallengeTracker

Short challenges, daily progress, public leaderboards. Users register, create or join time-boxed challenges ("read 100 pages this week"), log progress once per day, and see a live leaderboard.

Built as the WBS Software Engineering group project — fullstack ASP.NET Core 10 + React 19.

---

## Tech stack

| Layer | Choice |
|---|---|
| API | ASP.NET Core 10 Minimal API, EF Core, SQLite, ASP.NET Core Identity + JWT Bearer, Scalar/OpenAPI, ProblemDetails (RFC 7807) |
| Frontend | Vite + React 19 + TypeScript (strict), React Router v7, Tailwind v4 + DaisyUI, TanStack Query, Zod, react-toastify |
| Container | Multi-stage Dockerfile, Docker Compose for the local stack |

---

## Prerequisites

- .NET 10 SDK
- Node 20+
- Docker (only for the compose path)

---

## Quick start — Docker Compose

```bash
cp .env.example .env
# Edit .env: set JWT_KEY to a 64-character random secret.

docker compose up --build
```

The API is reachable at `http://localhost:8080`.
- `GET /health` — liveness
- `GET /ready` — readiness (DB + migrations)
- `GET /scalar/v1` — interactive API docs (development only)

The SQLite database is persisted in the named volume `challenge-data`. To wipe it: `docker compose down -v`.

> **Migrations.** The container does not run migrations on start. Before the first `compose up`, run `dotnet ef database update` once against the volume-mounted DB, or run the API once with the SDK locally to let EF create the schema.

---

## Quick start — manual (no Docker)

### Backend

```bash
cd backend/ChallengeTracker.Api
dotnet ef database update
dotnet run
```

API on `http://localhost:5290`. Scalar at `/scalar/v1`.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Frontend on `http://localhost:5173`. The dev server reads `VITE_API_BASE_URL` from `frontend/.env` (defaults to `http://localhost:5290`).

---

## Seed data

When the API runs in `Development`, a seeder creates two users and a public challenge:

| Email | Password | Role |
|---|---|---|
| `alice@example.com` | `Password1!` | Owner of "Read 100 pages this week" |
| `bob@example.com` | `Password1!` | Active member of Alice's challenge |

The seeder is idempotent — it skips if any challenge already exists.

---

## Endpoint overview

| Method | Path | Auth | Purpose |
|---|---|---|---|
| `POST` | `/auth/register` | No | Register a new user |
| `POST` | `/auth/login` | No | Get a JWT |
| `GET` | `/auth/me` | Yes | Current user |
| `GET` | `/challenges` | No | List with `?visibility=Public&status=Open` filters |
| `GET` | `/challenges/{id}` | No | Single challenge |
| `POST` | `/challenges` | Yes | Create (you become owner) |
| `POST` | `/challenges/{id}/start` | Yes (owner) | Open → Running |
| `POST` | `/challenges/{id}/complete` | Yes (owner) | Running → Completed |
| `POST` | `/memberships` | Yes | Join (Public → Active, Private → Pending) |
| `DELETE` | `/memberships/{id}` | Yes | Leave |
| `PATCH` | `/memberships/{id}` | Yes (owner) | Approve a Pending membership |
| `GET` | `/memberships/me/{challengeId}` | Yes | Caller's membership for this challenge |
| `POST` | `/progress-entries` | Yes (active member) | Log today's progress (1/day rule) |
| `PATCH` | `/progress-entries/{id}` | Yes (≤24h) | Edit |
| `DELETE` | `/progress-entries/{id}` | Yes (≤24h) | Remove |
| `GET` | `/progress-entries/me/today?challengeId=…` | Yes | Today's entry or 204 |
| `GET` | `/progress-entries/me?challengeId=…` | Yes | Personal history |
| `GET` | `/leaderboards/challenges/{id}?period=total&top=10` | No | Top-N standings |
| `GET` | `/health` | No | Liveness |
| `GET` | `/ready` | No | Readiness |

Write endpoints are rate-limited per user (10/min for progress logging, 5/min for everything else). Hitting the limit returns 429 with a `Retry-After` header.

---

## Demo flow

1. `docker compose up` (or `dotnet run` + `npm run dev`)
2. Open `http://localhost:5173`, log in as `alice@example.com / Password1!`
3. On the seeded challenge detail page → click **Start** → status flips to Running
4. Log out, log in as `bob@example.com / Password1!`
5. Detail page now shows **Leave** (Bob is an active member)
6. Submit a progress entry → toast confirms, leaderboard updates
7. Try to log a second entry the same day → 409 toast "You already logged for today"
8. Hammer the button quickly → 429 toast with retry-after hint

---

## Project structure

```
challenge-tracker/
├── backend/
│   └── ChallengeTracker.Api/
│       ├── .Api/{Endpoints,Filters,Dtos}/
│       ├── Application/{Interfaces,Services}/
│       ├── Infrastructure/{ApplicationDbContext.cs, Data/DbSeeder.cs}
│       ├── Models/
│       ├── Migrations/
│       ├── Program.cs
│       └── Dockerfile
├── frontend/
│   └── src/{components,contexts,data,layouts,pages,types,utils}/
├── docker-compose.yml
├── .env.example
└── README.md
```

---

## Troubleshooting

**`401` on every request after login.** Check that the token is in `localStorage` under `ct_token` and that `VITE_API_BASE_URL` points at the running API.

**`docker compose up` fails on EF migrations.** Run `dotnet ef database update` once locally before starting the container — the image doesn't apply migrations on boot.

**Frontend can't reach the API.** Confirm CORS allows your origin (`http://localhost:5173` is whitelisted in `Program.cs`). For other origins, edit the policy.

**Port 8080 already in use.** Edit the `ports:` mapping in `docker-compose.yml` (e.g. `8081:8080`).

---

## Team

Tracker Trio — built for the WBS Coding School Software Engineering group project.
