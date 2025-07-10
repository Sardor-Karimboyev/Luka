# Luka.Auth

Lightweight JWT authentication and permission management for the **Luka** .NET platform.

---

## ✨ Features

- 🔐 JWT token generation and validation
- ✅ Authorization via `[Authorize(Policy = "YOUR_PERMISSION")]`
- 🎯 Enum-based permission definition using `PermissionAttribute`

---

## ⚙️ Configuration

### `appsettings.json`

#### ✅ Minimal example

```json
{
  "jwt": {
    "issuerSigningKey": "yourSecretKey",
    "expiryMinutes": 60,
    "issuer": "lukachi",
    "validAudience": "lukachi",
    "validateAudience": false,
    "validateIssuer": false,
    "validateIssuerSigningKey": true,
    "validateLifetime": true,
    "isUsingSingleInstanceTokenRevocation": false
  }
}
```
---
## 🚀 Usage

1. Register JWT in Program.cs

🔴 With Redis token blacklist:
```csharp
builder.Services
    .AddLuka()
    .AddJwt<RedisAccessTokenService>() // Uses Redis for token revocation
    .AddRedis();
```
🟡 With In-Memory token blacklist:
```csharp
builder.Services
    .AddLuka()
    .AddJwt<InMemoryAccessTokenService>(); // Uses IMemoryCache for token revocation
```
⚪ Without token blacklist:
```csharp
builder.Services
    .AddLuka()
    .AddJwt(); // No token revocation
```
2. Enable middleware
```csharp
app.UseLuka()
   .UseAuthentication()           // Validates the JWT access token from the Authorization header
                                   // and sets HttpContext.User with claims from the token
   .UseAuthorization()           // Enforces access control based on registered policies and roles
                                   // (e.g., [Authorize(Policy = "CREATE_USER")])
   .UseAccessTokenValidator();   // Validates active token blacklist
```
---
## 🛠 Additional Notes
You can decorate your permission enum values with `[Permission]`, and `Luka.Auth` will automatically register them as policies and sync with the database.

Token revocation via Redis or in-memory is automatically handled depending on which service you register.

Logout endpoint can call ```IAccessTokenService.DeactivateCurrentAsync()```


