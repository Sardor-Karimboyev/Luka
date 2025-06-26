# Luka

a  simple recipe for .NET Core applications — inspired by DevMentors' Convey.

## 📦 Packages

- `Luka.Auth` – JWT generation, validation, permission policy system
- `Luka.Persistence.Redis` – Redis extensions
- `Luka.Persistence.Postgre` – PostgreSQL context setup and naming conventions
- `Luka.Docs.Swagger` – Swagger extensions
##  Install

```bash
dotnet add package Luka.Auth
 ```
##  Usage
```csharp
builder.Services.AddJwt&lt;
RedisAccessTokenService&gt;();
app.UseAuthentication().UseAuthorization().UseLuka();
```
##  Permissions
```csharp
[Permission] public enum Permission { CREATE_USER, DELETE_USER }
```
##  Publishing
Push a tag like `v1.0.0` to GitHub — CI/CD will automatically publish to NuGet.
##  License
MIT — free to use, modify, and distribute. </code></pre>
