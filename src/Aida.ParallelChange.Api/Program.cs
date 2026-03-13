using Aida.ParallelChange.Api.Application.GetCustomerContact;
using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer;
using Aida.ParallelChange.Api.Ports;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(sp => new DatabaseConnectionFactory(builder.Configuration.GetConnectionString("SqlServer")!));
builder.Services.AddScoped<SqlServerCustomerContactRepository>();
builder.Services.AddScoped<CustomerContactReader>(sp => sp.GetRequiredService<SqlServerCustomerContactRepository>());
builder.Services.AddScoped<CustomerContactWriter>(sp => sp.GetRequiredService<SqlServerCustomerContactRepository>());
builder.Services.AddScoped<GetCustomerContactHandler>();
builder.Services.AddScoped<UpdateCustomerContactHandler>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program;
