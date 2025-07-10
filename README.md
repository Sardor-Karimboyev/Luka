# Luka 🧩

**Luka** is a modular, opinionated platform for building modern, layered, and scalable .NET microservices.

This repository contains the source code and NuGet packages for all `Luka.*` components.

---

## 📦 Packages

| Package | Description |
|--------|-------------|
| [`Luka`](https://www.nuget.org/packages/Luka) | Core foundation: configuration, attributes, markers, and base types. |
| [`Luka.Auth`](https://www.nuget.org/packages/Luka.Auth) | Lightweight JWT authentication and policy-based authorization via enum permissions. |
| [`Luka.Persistence.Postgre`](https://www.nuget.org/packages/Luka.Persistence.Postgre) | PostgreSQL support with `DbContext`, `snake_case` naming, repositories, and migrations. |
| [`Luka.Persistence.Redis`](https://www.nuget.org/packages/Luka.Persistence.Redis) | Redis-based distributed caching and access token revocation support. |
| [`Luka.Docs.Swagger`](https://www.nuget.org/packages/Luka.Docs.Swagger) | Swagger (OpenAPI) documentation integration with enriched metadata and auth support. |

---

## 🚀 Getting Started

Each package has its own `README.md` with detailed instructions and usage examples.

Start with the root package:

```bash
dotnet add package Luka
```
Then explore and compose the platform:

```csharp
builder.Services
.AddLuka()
.AddJwt<InMemoryAccessTokenService>()
.AddPostgreSql<YourDbContext>()
.AddRepository();
```
```csharp
app.UseLuka()
.UseAuthentication()
.UseAuthorization()
.UseAccessTokenValidator()
.UseSwaggerDocs();
```
## 📚 Documentation
See individual package `README.md` files for full documentation.

Future full documentation may be published at: https://yourdomain.dev/luka

## 🛠 Build & Contribute
- IDE: Rider or Visual Studio 2022+

- CLI: .NET 8 SDK

- Contribution guidelines coming soon

## 📄 License
MIT

## 💬 Feedback & Support
Feel free to open issues or discussions for ideas, problems, or feature requests.
