using Microsoft.Extensions.Options;
using SchoolManagement.Core.Configuration;
using SchoolManagement.Core.Tenancy;

namespace SchoolManagement.Api.Middleware;

/// <summary>
/// Resolves the current tenant from the request domain (sent by the frontend in a
/// header, e.g. X-Tenant-Domain) and populates the scoped <see cref="ITenantContext"/>.
///
/// If the header is present but no active tenant matches, the request is rejected
/// with 400. If the header is absent the request continues unresolved — endpoints
/// that need a tenant will fail at data-access time, while tenant-agnostic endpoints
/// (health, tenant administration, swagger) still work.
/// </summary>
public sealed class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TenancyOptions _options;

    public TenantResolutionMiddleware(RequestDelegate next, IOptions<TenancyOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext, ITenantStore tenantStore)
    {
        var domain = ResolveDomain(context);

        if (!string.IsNullOrWhiteSpace(domain))
        {
            var tenant = await tenantStore.GetByDomainAsync(domain.Trim().ToLowerInvariant(), context.RequestAborted);
            if (tenant is null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = $"Unknown or inactive tenant domain: '{domain}'." });
                return;
            }

            tenantContext.SetTenant(tenant);
        }

        await _next(context);
    }

    private string? ResolveDomain(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(_options.HeaderName, out var headerValue))
        {
            var value = headerValue.ToString();
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        if (_options.AllowQueryStringOverride &&
            context.Request.Query.TryGetValue("tenant", out var queryValue) &&
            !string.IsNullOrWhiteSpace(queryValue))
        {
            return queryValue.ToString();
        }

        return null;
    }
}
