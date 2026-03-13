using Aida.ParallelChange.Api.Application.GetCustomerContact;
using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Aida.ParallelChange.Api.Infrastructure.InMemory;
using Aida.ParallelChange.Api.Ports;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Aida.ParallelChange.Api.Tests.Acceptance;

public sealed class TestApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<InMemoryCustomerContactRepository>();
            services.AddSingleton<CustomerContactReader>(sp => sp.GetRequiredService<InMemoryCustomerContactRepository>());
            services.AddSingleton<CustomerContactWriter>(sp => sp.GetRequiredService<InMemoryCustomerContactRepository>());
            services.AddScoped<GetCustomerContactHandler>();
            services.AddScoped<UpdateCustomerContactHandler>();
        });
    }
}
