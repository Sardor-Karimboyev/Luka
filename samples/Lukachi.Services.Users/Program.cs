using System.Security.Claims;
using Luka;
using Luka.Auth;
using Luka.Auth.Middlewares;
using Luka.Docs.Swagger;
using Luka.Persistence.Postgre;
using Luka.Persistence.Postgre.Repositories;
using Luka.Persistence.Redis;
using Lukachi.Services.Users.Auth;
using Lukachi.Services.Users.Commands;
using Lukachi.Services.Users.Entities;
using Lukachi.Services.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IJwtProvider, JwtProvider>();
builder.Services
    .AddLuka()
    .AddSwaggerDocs()
    .AddJwt<RedisAccessTokenService>()
    .AddPostgreSql<UsersDbContext>()
    .AddRepository()
    .AddRedis();
    
var app = builder.Build();

app.UseLuka()
    .UseSwaggerDocs()
    .UseHttpsRedirection()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseAccessTokenValidator()
    .UsePostgreSql<UsersDbContext>()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapGet("", () => "Lukachi Services Users");
        endpoints.MapGet("ping", () => "pong");
        endpoints.MapPost("login", (IJwtProvider _jwtProvider,[FromBody] Login login) =>
        {
            var customClaims = new Dictionary<string, IEnumerable<string>>
            {
                {"permission", new[] {"CREATE_USER", "GET_USER"}},
            };
            var token = _jwtProvider.Create("123", "admin", null, customClaims);
            return token;
        });
        endpoints.MapPost("logout", [Authorize] async (
            IAccessTokenService tokenService) =>
        {
            await tokenService.DeactivateCurrentAsync();
            return Results.Ok(new { message = "Logged out successfully." });
        });
        
        endpoints.MapPost("users/", [Authorize(Policy = "BROWSE_USERS")] async (UsersDbContext _dbContext, [FromBody] CreateUser createUser) =>
        {
            var user = new User() {Username = createUser.Username, CreatedAt = DateTime.Now};
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return Results.Ok(new { id = user.Id });
        });
        endpoints.MapPost("users-repo/", [Authorize(Policy = "CREATE_USER")] async (IRepository<User, int> _userRepository, [FromBody] CreateUser createUser) =>
        {
            var user = new User() {Username = createUser.Username, CreatedAt = DateTime.Now};
            await _userRepository.InsertAsync(user);
            
            return Results.Ok(new { id = user.Id });
        });
        endpoints.MapGet("auth-test", [Authorize]() => "success");
    });

await app.RunAsync();