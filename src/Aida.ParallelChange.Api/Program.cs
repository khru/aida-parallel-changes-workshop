using Aida.ParallelChange.Api.Controllers.V1;
using Aida.ParallelChange.Api.Infrastructure.InMemory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<LegacyCustomerContactStore>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program;
