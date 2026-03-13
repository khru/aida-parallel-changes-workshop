using Aida.ParallelChange.Api.Application.GetCustomerContact;
using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Aida.ParallelChange.Api.Infrastructure.InMemory;
using Aida.ParallelChange.Api.Ports;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<InMemoryCustomerContactRepository>();
builder.Services.AddSingleton<CustomerContactReader>(sp => sp.GetRequiredService<InMemoryCustomerContactRepository>());
builder.Services.AddSingleton<CustomerContactWriter>(sp => sp.GetRequiredService<InMemoryCustomerContactRepository>());
builder.Services.AddScoped<GetCustomerContactHandler>();
builder.Services.AddScoped<UpdateCustomerContactHandler>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program;
