using Aida.ParallelChange.Api.Application.CreateCustomerContact;
using Aida.ParallelChange.Api.Application.GetCustomerContact;
using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Aida.ParallelChange.Api.Infrastructure.Http.Errors;
using Aida.ParallelChange.Api.Infrastructure.OpenApi;
using Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(sp => new DatabaseConnectionFactory(builder.Configuration.GetConnectionString("SqlServer")!));
builder.Services.AddScoped<SqlServerCustomerContactRepository>();
builder.Services.AddScoped<CustomerContactReader>(sp => sp.GetRequiredService<SqlServerCustomerContactRepository>());
builder.Services.AddScoped<CustomerContactCreator>(sp => sp.GetRequiredService<SqlServerCustomerContactRepository>());
builder.Services.AddScoped<CustomerContactUpdater>(sp => sp.GetRequiredService<SqlServerCustomerContactRepository>());
builder.Services.AddScoped<CreateCustomerContactHandler>();
builder.Services.AddScoped<GetCustomerContactHandler>();
builder.Services.AddScoped<UpdateCustomerContactHandler>();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<IJsonApiExceptionMapper, ApiRequestValidationJsonApiExceptionMapper>();
builder.Services.AddSingleton<IJsonApiExceptionMapper, CustomerContactAlreadyExistsJsonApiExceptionMapper>();
builder.Services.AddSingleton<IJsonApiExceptionMapper, CustomerContactNotFoundJsonApiExceptionMapper>();
builder.Services.AddSingleton<IJsonApiExceptionMapper, BadHttpRequestJsonApiExceptionMapper>();
builder.Services.AddSingleton<IJsonApiExceptionMapper, UnexpectedJsonApiExceptionMapper>();
builder.Services.AddSingleton<JsonApiExceptionMapperFactory>();
builder.Services.AddExceptionHandler<JsonApiExceptionHandler>();
builder.Services.AddWorkshopOpenApi();

var app = builder.Build();

app.UseWorkshopOpenApi();
app.UseExceptionHandler();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program;
