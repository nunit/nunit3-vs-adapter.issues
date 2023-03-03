using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;

namespace lib;

public static class Class1
{
    public static void MapHealthChecks(IEndpointRouteBuilder builder)
    {
        builder.MapHealthChecks("/foo", new HealthCheckOptions { Predicate = check => check.Tags.Contains("foo") });
    }
}