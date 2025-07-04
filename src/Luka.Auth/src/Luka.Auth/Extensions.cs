﻿using System.Security.Cryptography.X509Certificates;
using System.Text;
using Luka.Auth.Handlers;
using Luka.Auth.Middlewares;
using Luka.Auth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Luka.Auth;

public static class Extensions
{
    private const string SectionName = "jwt";
    private const string RegistryName = "auth";


    public static ILukaBuilder AddJwt<TAccessTokenService>(this ILukaBuilder builder, string sectionName = SectionName,
        Action<JwtBearerOptions> optionsFactory = null)
    {
        if (typeof(TAccessTokenService) == typeof(InMemoryAccessTokenService))
            builder.Services.AddMemoryCache();

        builder.Services.AddScoped(typeof(IAccessTokenService), typeof(TAccessTokenService));
        return builder.AddJwt(sectionName, optionsFactory);
    }

    public static ILukaBuilder AddJwt(this ILukaBuilder builder, string sectionName = SectionName,
        Action<JwtBearerOptions> optionsFactory = null)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        var options = builder.GetOptions<JwtOptions>(sectionName);
        return builder.AddJwt(options, optionsFactory);
    }

    private static ILukaBuilder AddJwt(this ILukaBuilder builder, JwtOptions options,
        Action<JwtBearerOptions> optionsFactory = null)
    {
        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddSingleton<IJwtHandler, JwtHandler>();
        if(options.IsUsingSingleInstanceTokenRevocation)
            builder.Services.AddSingleton<IAccessTokenService, InMemoryAccessTokenService>();
       // builder.Services.AddTransient<AccessTokenValidatorMiddleware>();

        if (options.AuthenticationDisabled)
        {
            //builder.Services.AddSingleton<IPolicyEvaluator, DisabledAuthenticationPolicyEvaluator>();
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            RequireAudience = options.RequireAudience,
            ValidIssuer = options.ValidIssuer,
            ValidIssuers = options.ValidIssuers,
            ValidateActor = options.ValidateActor,
            ValidAudience = options.ValidAudience,
            ValidAudiences = options.ValidAudiences,
            ValidateAudience = options.ValidateAudience,
            ValidateIssuer = options.ValidateIssuer,
            ValidateLifetime = options.ValidateLifetime,
            ValidateTokenReplay = options.ValidateTokenReplay,
            ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,
            SaveSigninToken = options.SaveSigninToken,
            RequireExpirationTime = options.RequireExpirationTime,
            RequireSignedTokens = options.RequireSignedTokens,
            ClockSkew = TimeSpan.Zero
        };

        if (!string.IsNullOrWhiteSpace(options.AuthenticationType))
        {
            tokenValidationParameters.AuthenticationType = options.AuthenticationType;
        }

        var hasCertificate = false;
        if (options.Certificate is not null)
        {
            X509Certificate2 certificate = null;
            var password = options.Certificate.Password;
            var hasPassword = !string.IsNullOrWhiteSpace(password);
            if (!string.IsNullOrWhiteSpace(options.Certificate.Location))
            {
                certificate = hasPassword
                    ? new X509Certificate2(options.Certificate.Location, password)
                    : new X509Certificate2(options.Certificate.Location);
                var keyType = certificate.HasPrivateKey ? "with private key" : "with public key only";
                Console.WriteLine($"Loaded X.509 certificate from location: '{options.Certificate.Location}' {keyType}.");
            }
                
            if (!string.IsNullOrWhiteSpace(options.Certificate.RawData))
            {
                var rawData = Convert.FromBase64String(options.Certificate.RawData);
                certificate = hasPassword
                    ? new X509Certificate2(rawData, password)
                    : new X509Certificate2(rawData);
                var keyType = certificate.HasPrivateKey ? "with private key" : "with public key only";
                Console.WriteLine($"Loaded X.509 certificate from raw data {keyType}.");
            }

            if (certificate is not null)
            {
                if (string.IsNullOrWhiteSpace(options.Algorithm))
                {
                    options.Algorithm = SecurityAlgorithms.RsaSha256;
                }

                hasCertificate = true;
                tokenValidationParameters.IssuerSigningKey = new X509SecurityKey(certificate);
                var actionType = certificate.HasPrivateKey ? "issuing" : "validating";
                Console.WriteLine($"Using X.509 certificate for {actionType} tokens.");
            }
        }

        if (!string.IsNullOrWhiteSpace(options.IssuerSigningKey) && !hasCertificate)
        {
            if (string.IsNullOrWhiteSpace(options.Algorithm) || hasCertificate)
            {
                options.Algorithm = SecurityAlgorithms.HmacSha256;
            }

            var rawKey = Encoding.UTF8.GetBytes(options.IssuerSigningKey);
            tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(rawKey);
            Console.WriteLine("Using symmetric encryption for issuing tokens.");
        }

        if (!string.IsNullOrWhiteSpace(options.NameClaimType))
        {
            tokenValidationParameters.NameClaimType = options.NameClaimType;
        }

        if (!string.IsNullOrWhiteSpace(options.RoleClaimType))
        {
            tokenValidationParameters.RoleClaimType = options.RoleClaimType;
        }

        var permissionEnums = PermissionScanner.FindPermissionEnums();
        if (permissionEnums.Any())
        {
            builder.Services.AddAuthorization(options =>
            {
                foreach (var enumType in permissionEnums)
                {
                    var permissions = PermissionScanner.GetPermissionsFromEnum(enumType);

                    foreach (var permission in permissions)
                    {
                        options.AddPolicy(permission, policy =>
                            policy.RequireClaim("permission", permission));
                    }
                }
            });
        }

        builder.Services
            .AddAuthorization()
            .AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.Authority = options.Authority;
                o.Audience = options.Audience;
                o.MetadataAddress = options.MetadataAddress;
                o.SaveToken = options.SaveToken;
                o.RefreshOnIssuerKeyNotFound = options.RefreshOnIssuerKeyNotFound;
                o.RequireHttpsMetadata = options.RequireHttpsMetadata;
                o.IncludeErrorDetails = options.IncludeErrorDetails;
                o.TokenValidationParameters = tokenValidationParameters;
                if (!string.IsNullOrWhiteSpace(options.Challenge))
                {
                    o.Challenge = options.Challenge;
                }

                optionsFactory?.Invoke(o);
            });

        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton(tokenValidationParameters);

        return builder;
    }

  public static IApplicationBuilder UseAccessTokenValidator(this IApplicationBuilder app)
      => app.UseMiddleware<AccessTokenRevocationMiddleware>();
    
    public static long ToTimestamp(this DateTime dateTime) => new DateTimeOffset(dateTime).ToUnixTimeSeconds();
    
}