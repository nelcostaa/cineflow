# CineflowFront - React Frontend

Frontend do sistema Cineflow. React 19 + Vite (build tool).

## Quick Start

### 1. Build

```bash
npm ci
npm run build
```

### 2. Run (via Docker Compose, from project root)

```bash
docker-compose up -d
```

- Frontend: http://localhost
- API: http://localhost/api
- Swagger: http://localhost/swagger

## Architecture

```
Browser (port 80)
    |
nginx (single container)
    |-- /api/*     --> CineFlowAPI:8080
    |-- /swagger   --> CineFlowAPI:8080/swagger
    '-- /*         --> React build (dist/)
```

Nginx serves the React build directly from `dist/` and proxies API requests.
No separate frontend container -- single nginx handles everything.

## Configuration

| Variable       | File                                   | Default                |
| -------------- | -------------------------------------- | ---------------------- |
| `VITE_API_URL` | `.env.development` / `.env.production` | `http://localhost/api` |

Override locally with `.env.local` (git-ignored).

## Project Structure

```
src/
  components/   FilmeModal, IngressoModal, Navbar
  pages/        Home, Filmes, Sessoes, CriarSessao, Salas, Seed
  services/     api.js (centralized API client)
  assets/       Static resources
```

## Routes

| Path            | Component   | Description        |
| --------------- | ----------- | ------------------ |
| `/`             | Home        | Landing page       |
| `/filmes`       | Filmes      | Movie catalog      |
| `/sessoes`      | Sessoes     | Session schedule   |
| `/criar-sessao` | CriarSessao | Create new session |
| `/salas`        | Salas       | Room management    |
| `/seed`         | Seed        | Test data seeding  |

Uses `BrowserRouter` (clean URLs). Nginx `try_files` handles deep linking and page refresh.

## Scripts

```bash
npm run dev      # Vite dev server (port 5173) -- for local development only
npm run build    # Production build --> dist/
npm run preview  # Preview production build locally
npm run lint     # ESLint
```
