# ChallengeTracker

Full-stack habit tracker: create short challenges, join them, log daily progress, compete on a leaderboard.

Built by **Tracker Trio** — Rud, Umi, Max.

## Stack

- **Backend:** ASP.NET Core 10 Minimal API, EF Core, SQLite, JWT
- **Frontend:** Vite, React 19, TypeScript, Tailwind CSS, DaisyUI

## Structure

```
backend/ChallengeTracker.Api   # .NET API
frontend                        # React SPA
scripts/init.sh                 # one-time scaffold
```

## First-time setup

```bash
./scripts/init.sh
```

This creates the backend and frontend projects and installs dependencies.

## Run locally

```bash
# API
cd backend/ChallengeTracker.Api
dotnet ef database update
dotnet run
# → http://localhost:5290
# → Scalar UI at http://localhost:5290/scalar/v1

# Frontend (new terminal)
cd frontend
cp .env.example .env
npm run dev
# → http://localhost:5173
```

## Team

- **Rud** — Backend core & auth (EF Core, Identity, JWT, infrastructure)
- **Umi** — Backend features (Challenges, Memberships, Progress, Leaderboard)
- **Max** — Frontend (React app, auth flow, dashboard, API integration)
