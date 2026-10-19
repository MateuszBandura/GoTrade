# GoTrade

A full-stack web application for trading Pokémon and costumes: an ASP.NET Core Web API backend (C#) with an Angular frontend (TypeScript) that lets users register, manage profiles, and trade Pokémon/costumes.

## Stack
- Language(s): C#, TypeScript
- Framework / runtime: ASP.NET Core Web API (C#) + Angular (CLI / runtime)
- Notable libraries and patterns: Entity Framework Core (DbContext & Migrations), typical ASP.NET controllers/DTO pattern, Angular CLI for the client

## Project layout
```
api/                     ← ASP.NET Core Web API project
  Program.cs             ← application entry
  api.csproj             ← .NET project file (dependencies & target framework)
  appsettings.Development.json
  api.http               ← example HTTP requests
  Controllers/           ← REST controllers (e.g. AccountController)
  DTOs/                  ← request/response DTOs (LoginDto, RegisterDto, UserDto)
  Data/                  ← DbContext, Migrations, import utilities
    AppDbContext.cs
    Migrations/
  Entities/              ← domain entities (AppUser, Pokemon, Trainer, Costume, etc.)
  Services/              ← business logic and trade services
  Interfaces/            ← service interfaces
  Extensions/            ← helper/extension methods
client/                  ← Angular frontend
  angular.json
  package.json
  package-lock.json
  src/
    index.html
    main.ts
    styles.css
    app/                  ← Angular app components, services, modules (src/app)
  README.md               ← client-specific README and instructions
```

How it fits together:
- The Angular client (client/) calls the ASP.NET Core Web API (api/) for authentication and trading operations.
- The API exposes REST endpoints implemented in Controllers, maps network models via DTOs, and persists domain entities through an EF Core AppDbContext (Migrations folder present).
- Services encapsulate business logic (trading, user management) and are wired into controllers via interfaces and dependency injection.

## Quick start — run locally

Prerequisites
- .NET SDK (compatible with the project's target framework)
- Node.js + npm (for the Angular client)
- A database (configure connection string in api/appsettings.Development.json or via environment variables). The project contains EF Core migrations.

Start the backend (api)
```bash
# from repository root
cd api

# restore and build
dotnet restore
dotnet build

# apply EF Core migrations (optional, requires dotnet-ef tools)
# dotnet tool install --global dotnet-ef   # if you don't have it
dotnet ef database update

# run the API
dotnet run
# or: dotnet run --project api.csproj
```

Notes:
- Check `api/appsettings.Development.json` for the default connection string placeholder; set `ConnectionStrings:DefaultConnection` (or equivalent) to point to your DB.
- If the project uses environment configuration, set ASPNETCORE_ENVIRONMENT=Development to pick up development settings.

Start the frontend (client)
```bash
cd client

# install dependencies
npm install

# start dev server (common scripts: npm start / npm run start)
npm start
# or (if you have Angular CLI globally):
ng serve --open
```

Run both:
- In development, run api and client in separate terminals. Configure CORS or a proxy if needed so the Angular app can call the API.

API testing
- The `api/api.http` file (in the api folder) contains sample HTTP requests you can run with tools like VS Code REST Client or Postman. Use it to exercise authentication and basic endpoints.

## Development notes
- Database access is implemented through `api/Data/AppDbContext.cs`. Look at the `Entities/` models (Pokemon, Trainer, Costume, AppUser) to understand the domain model.
- Business logic for trades and user operations likely lives under `api/Services/` and is surfaced via controller endpoints.
- The Angular client entry points are `client/src/main.ts` and `client/src/index.html`; components and services are under `client/src/app`.

## Contributing
- Open an issue describing your change or proposed feature before sending a PR.
- Include migrations and tests for any database schema or business-logic changes.
- Add/update documentation in this README or `client/README.md` for frontend-specific instructions.

## Tests
- No top-level test runner detected in the repo root; check the `client` and `api` folders for any test projects or scripts. Add test suites and test scripts if required.

## License
- No license file detected in the repository. Add a LICENSE file (for example, MIT) to clarify reuse and contribution rules.

## Where to look next
- api/Controllers — available endpoints and route shapes
- api/Data — DbContext and migrations
- client/README.md — frontend-specific developer notes and scripts
- api/api.http — example requests for quick manual testing

## Try asking
- Where are trading rules and match/offer logic implemented (I looked for trade-related code under api/Services)?
- Does api/api.http include authentication and sample trade requests, and can you add Postman collection equivalents?
- Is there a recommended DB provider (SQLite, SQL Server, etc.) in appsettings.Development.json or in the project docs?

If you want, I can produce a ready-to-drop README.md file (with badges, example endpoints, and a short contributing guide) tailored to this repo — tell me whether to include example cURL requests or a Docker compose for local development.
