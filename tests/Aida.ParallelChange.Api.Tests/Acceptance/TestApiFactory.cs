using Aida.ParallelChange.Api.Application.CreateCustomerContact;
using Aida.ParallelChange.Api.Application.GetCustomerContact;
using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Aida.ParallelChange.Api.Infrastructure.InMemory;
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
            services.AddSingleton<CustomerContactCreator>(sp => sp.GetRequiredService<InMemoryCustomerContactRepository>());
            services.AddSingleton<CustomerContactUpdater>(sp => sp.GetRequiredService<InMemoryCustomerContactRepository>());
            services.AddScoped<CreateCustomerContactHandler>();
            services.AddScoped<GetCustomerContactHandler>();
            services.AddScoped<UpdateCustomerContactHandler>();
        });
    }
}
