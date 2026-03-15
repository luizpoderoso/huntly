# Huntly

A full-stack job application tracker built to demonstrate production-grade software engineering practices — Clean Architecture, CQRS, Domain-Driven Design patterns, and a modern reactive frontend.

> This is a portfolio project. The goal is not just a working app, but a codebase that reflects the kind of decisions made in real, maintainable production systems.

---

## What it does

Huntly lets you track job applications through their lifecycle — from initial application to offer or rejection. Each application can have interviews and notes attached to it, and the status updates in real time.

---

## Stack

### Backend
| Technology | Reason |
|---|---|
| **.NET 10** | Latest LTS, performance improvements, primary language expertise |
| **FastEndpoints** | Replaces MVC controllers — thinner endpoints, better performance, explicit request/response types |
| **MediatR** | CQRS pipeline — commands and queries are decoupled from their handlers |
| **EF Core + PostgreSQL** | Mature ORM with strong migration tooling, PostgreSQL for production-grade relational storage |
| **ASP.NET Core Identity** | Industry-standard auth primitives without reinventing the wheel |
| **Argon2id** | Replaces Identity's default PBKDF2 — memory-hard, resistant to GPU brute force attacks |

### Frontend
| Technology | Reason |
|---|---|
| **SvelteKit** | File-based routing, SSR-ready, minimal boilerplate |
| **Svelte 5 (runes)** | Latest Svelte — signals-based reactivity, cleaner state management |
| **TypeScript** | Type safety across API contracts and component props |
| **shadcn-svelte** | Accessible, unstyled-by-default components — owned code, not a dependency |
| **Tailwind CSS** | Utility-first, consistent spacing and theming |

---

## Architecture

Huntly follows Clean Architecture with four layers. The critical invariant is that `Core` has zero dependencies on any other layer — the domain never knows about the database, HTTP, or any framework.

```
┌──────────┐     ┌─────────────┐     ┌──────┐
│   Api    │────▶│ Application │────▶│ Core │
└────┬─────┘     └─────────────┘     └──────┘
     │                  ▲                 ▲
     │           ┌──────┴───┐            │
     └──────────▶│  Infra   │────────────┘
                 └──────────┘
```

`Api` references `Infra` only at the composition root — `Program.cs` calls `AddInfrastructure()` to wire up DI. Nothing else in `Api` (endpoints, middleware) ever touches `Infra` directly. Endpoints only talk to `Application` via MediatR.

`Infra` depends on both `Application` and `Core`, but for different reasons:
- **`Infra → Core`** — implements `IJobApplicationRepository` (defined in `Core`) and references domain entities directly in EF Core configurations. Repository interfaces live in `Core` because the domain defines what persistence it needs — infrastructure provides it.
- **`Infra → Application`** — implements `ITokenService`, `IAtomicWork`, and `IUserContext` (defined in `Application`). These are orchestration abstractions, not domain concerns — the domain has no concept of JWT tokens or units of work.

What matters is that the arrow never reverses: `Core` knows nothing about EF Core, PostgreSQL, or any infrastructure concern.

### Key design decisions

**Value Objects over primitive types**
`CompanyName`, `Position`, `JobUrl`, and `SalaryRange` are Value Objects, not strings and decimals. Rules are enforced at construction — if the object exists, it is valid. No scattered null checks or format validation across the codebase.

**CQRS with MediatR pipeline behaviors**
Every use case is a `Command` or `Query` with a dedicated `Handler`. Cross-cutting concerns — validation, logging, performance monitoring — run as pipeline behaviors registered once, applied to every request automatically.

**Argon2id password hashing**
Identity's default PBKDF2 is vulnerable to GPU-accelerated attacks. Argon2id is memory-hard by design, making brute force attacks impractical even with modern hardware. Password validation rules live in `RegisterCommandValidator`, not in Identity — a single source of truth.

**Aggregate boundary enforcement**
`Interview` and `Note` are child entities of `JobApplication`. They have no navigation property back to the parent and are only reachable through the aggregate root. EF Core manages the foreign key as a shadow property — it exists in the database but not in the domain model.

**Separation of mutation errors from fetch state**
In the frontend jobs store, failed mutations (create, update, delete) surface as toast notifications only. They never overwrite the `error` state used by the UI — a failed form submission never hides your job list.

---

## Project structure

```
Huntly/
├── backend/
│   └── src/
│       ├── Huntly.Core/         # Entities, VOs, enums, repository interfaces
│       ├── Huntly.Application/  # CQRS handlers, validators, behaviors, DTOs
│       ├── Huntly.Infra/        # EF Core, Identity, JWT, Argon2
│       └── Huntly.Api/          # FastEndpoints, middleware, Program.cs
└── frontend/
    └── src/
        ├── lib/
        │   ├── api/
        │   ├── assets/
        │   ├── components/      # shadcn-svelte + custom components
        │   ├── hooks/           # Generated by shadcn-svelte sidebar
        │   ├── stores/          # Svelte 5 rune-based state
        │   ├── types/
        │   ├── index.ts
        │   └── utils.ts
        └── routes/
            ├── +layout.svelte
            ├── (auth)/          # Login, register — centered card layout
            └── (app)/           # Dashboard, job detail — sidebar layout
```

---

## Getting started

### Prerequisites
- .NET 10 SDK
- Node.js 20+ and pnpm
- PostgreSQL (or Docker)

### Backend

**1. Clone and navigate to backend**
```bash
git clone https://github.com/your-username/huntly.git
cd huntly/backend
```

**2. Set up User Secrets**
```bash
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Database=huntly;Username=postgres;Password=your-password" -p src/Huntly.Api
dotnet user-secrets set "Jwt:Secret" "your-secret-min-32-characters-long" -p src/Huntly.Api
dotnet user-secrets set "Auth:Pepper" "your-pepper-value" -p src/Huntly.Api
```

**3. Run migrations**
```bash
dotnet ef database update --project src/Huntly.Infra --startup-project src/Huntly.Api
```

**4. Start the API**
```bash
dotnet run --project src/Huntly.Api
```

The API runs on `http://localhost:5163`.

### Frontend

**1. Navigate to frontend**
```bash
cd huntly/frontend
```

**2. Install dependencies**
```bash
pnpm install
```

**3. Set up environment**
```bash
echo "PUBLIC_API_URL=http://localhost:5163/api" > .env
```

**4. Start the dev server**
```bash
pnpm dev
```

The frontend runs on `http://localhost:5173`.

---

## API overview

All endpoints are prefixed with `/api`.

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/auth/register` | Register a new account |
| `POST` | `/api/auth/login` | Login and receive JWT |
| `GET` | `/api/jobs` | List all job applications |
| `POST` | `/api/jobs` | Create a job application |
| `GET` | `/api/jobs/:id` | Get job detail with interviews and notes |
| `PATCH` | `/api/jobs/:id/status` | Update application status |
| `DELETE` | `/api/jobs/:id` | Delete a job application |

---

## Roadmap

- [ ] Domain events + SignalR real-time notifications
- [ ] Interview and note management
- [ ] Dashboard statistics (applied / interviewing / offers / rejections)
- [ ] Filters and search
- [ ] Unit tests for Application handlers
- [ ] Integration tests for API endpoints
- [ ] Docker + docker-compose for one-command setup

---

## License

MIT