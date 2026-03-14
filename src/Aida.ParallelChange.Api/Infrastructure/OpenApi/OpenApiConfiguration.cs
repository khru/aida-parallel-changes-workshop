using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;

namespace Aida.ParallelChange.Api.Infrastructure.OpenApi;

public static class OpenApiConfiguration
{
    public static IServiceCollection AddWorkshopOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi("v1");
        return services;
    }

    public static WebApplication UseWorkshopOpenApi(this WebApplication app)
    {
        app.MapOpenApi("/openapi/{documentName}.json");
        return app;
    }
}
