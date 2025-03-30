using FinancialServices.Domain.Security.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace FinancialServices.Api.Middleware;

public class ApiKeyRequestHandlerMiddleware(RequestDelegate next)
{
    private const string APIKEY_HEADER = "x-api-key";
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, IAuthUserUseCase authUserUseCase, IAuthorizationPolicyProvider policyProvider)
    {
        var path = context.Request.Path.Value;

        // Ignora autenticação para rotas fora do /api
        if (!path.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Verifica se a API Key está presente
        if (!context.Request.Headers.TryGetValue(APIKEY_HEADER, out var key) || string.IsNullOrWhiteSpace(key))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key is missing");
            return;
        }

        var apiKey = key.First()!;
        var requiredRoles = await GetRequiredRolesAsync(context, policyProvider);

        var user = authUserUseCase.AuthUser(apiKey, requiredRoles);

        if (user == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.GivenName, user.UserName),
            new("ApiKey", user.ApiKey)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "ApiKey"));
        await _next(context);
    }

    private static async Task<string[]> GetRequiredRolesAsync(HttpContext context, IAuthorizationPolicyProvider policyProvider)
    {
        var endpoint = context.GetEndpoint();
        var authorizeAttributes = endpoint?.Metadata.GetOrderedMetadata<AuthorizeAttribute>();

        if (authorizeAttributes == null || !authorizeAttributes.Any())
            return [];

        var roles = new List<string>();

        foreach (var attr in authorizeAttributes)
        {
            if (!string.IsNullOrWhiteSpace(attr.Policy))
            {
                var policy = await policyProvider.GetPolicyAsync(attr.Policy);
                if (policy != null)
                {
                    var policyRoles = policy.Requirements
                        .OfType<RolesAuthorizationRequirement>()
                        .SelectMany(r => r.AllowedRoles);
                    roles.AddRange(policyRoles);
                }
            }

            if (!string.IsNullOrWhiteSpace(attr.Roles))
            {
                var inlineRoles = attr.Roles
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                roles.AddRange(inlineRoles);
            }
        }

        return roles.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
    }
}
