#!/usr/bin/env bash
#
# One-time scaffold for the ChallengeTracker project.
# Run once from the repo root after cloning — creates the backend and frontend
# projects with their dependencies. Safe to re-run (skips steps if files exist).
#

set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
BACKEND_DIR="$ROOT/backend/ChallengeTracker.Api"
FRONTEND_DIR="$ROOT/frontend"

echo "==> ChallengeTracker scaffold"
echo "    root: $ROOT"

# ---------- Backend ----------
if [ ! -f "$BACKEND_DIR/ChallengeTracker.Api.csproj" ]; then
  echo "==> Creating backend project (Minimal API)"
  mkdir -p "$ROOT/backend"
  cd "$ROOT/backend"
  dotnet new web -o ChallengeTracker.Api --framework net10.0
  cd "$BACKEND_DIR"

  echo "==> Adding backend packages"
  dotnet add package Microsoft.AspNetCore.OpenApi
  dotnet add package Microsoft.EntityFrameworkCore
  dotnet add package Microsoft.EntityFrameworkCore.Sqlite
  dotnet add package Microsoft.EntityFrameworkCore.Design
  dotnet add package Scalar.AspNetCore

  echo "==> Creating solution file"
  cd "$ROOT/backend"
  dotnet new sln -n ChallengeTracker
  dotnet sln add ChallengeTracker.Api/ChallengeTracker.Api.csproj
else
  echo "==> Backend project already exists — skipping"
fi

# ---------- Frontend ----------
if [ ! -f "$FRONTEND_DIR/package.json" ]; then
  echo "==> Creating frontend project (Vite + React + TS)"
  cd "$ROOT"
  rm -rf "$FRONTEND_DIR"
  npm create vite@latest frontend -- --template react-ts
  cd "$FRONTEND_DIR"

  echo "==> Installing frontend dependencies"
  npm install
  npm install react-router-dom @tanstack/react-query zod react-toastify
  npm install -D tailwindcss @tailwindcss/vite daisyui
else
  echo "==> Frontend project already exists — skipping"
fi

echo "==> Done. Next steps:"
echo "    1) Copy the template files from /_templates over the generated code"
echo "    2) cd backend/ChallengeTracker.Api && dotnet ef migrations add InitialCreate"
echo "    3) cd frontend && npm run dev"
