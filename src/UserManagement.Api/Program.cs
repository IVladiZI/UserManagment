using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System.Reflection;
using UserManagement.Api.Abstractions;
using UserManagement.Api.Configuration;
using UserManagement.Api.Middlewares;
using UserManagement.Api.Observability;
using UserManagement.Api.ProblemHandling;
using UserManagement.Application.Users.Commands.RegisterUser;
using UserManagement.Domain.Repositories;
using UserManagement.Infrastructure.Data;
using UserManagement.Infrastructure.Repositories;
using UserManagement.Application.Common.Mediator;
using UserManagement.Infrastructure.Mediator;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

// Swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "User Management API", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddControllers();

// Bind de opciones tipadas para Npgsql/EF
builder.Services.Configure<DatabaseNpgsqlOptions>(
    builder.Configuration.GetSection(DatabaseNpgsqlOptions.SectionName));

// DbContext usando IOptions<DatabaseNpgsqlOptions>
builder.Services.AddDbContext<UserManagementDbContext>((sp, options) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var dbOpts = sp.GetRequiredService<IOptions<DatabaseNpgsqlOptions>>().Value;
    var connString = configuration.GetConnectionString("DefaultConnection");

    options.UseNpgsql(connString, npgsql =>
    {
        npgsql.CommandTimeout(dbOpts.CommandTimeoutSeconds);

        if (dbOpts.EnableRetryOnFailure)
        {
            npgsql.EnableRetryOnFailure(
                maxRetryCount: dbOpts.MaxRetryCount,
                maxRetryDelay: TimeSpan.FromSeconds(dbOpts.MaxRetryDelaySeconds),
                errorCodesToAdd: null);
        }
    });

    options.EnableDetailedErrors(dbOpts.EnableDetailedErrors);
    options.EnableSensitiveDataLogging(dbOpts.EnableSensitiveDataLogging);
});

// Infra & MediatR & Middlewares
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IExceptionToProblemDetailsMapper, ExceptionToProblemDetailsMapper>();
builder.Services.AddSingleton<IErrorResponseWriter, ErrorResponseWriter>();
builder.Services.AddScoped<IRequestResponseLogger, RequestResponseLogger>();
builder.Services.AddTransient<ProblemDetailsMiddleware>();

// Registrar Mediator
builder.Services.AddSingleton<ISender, Mediator>();

// Registrar automáticamente todos los IRequestHandler<> del ensamblado Application
var appAssembly = typeof(RegisterUserCommandHandler).Assembly;
foreach (var type in appAssembly.GetTypes())
{
    var handlerInterfaces = type.GetInterfaces()
        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(UserManagement.Application.Common.Mediator.IRequestHandler<,>));
    foreach (var @interface in handlerInterfaces)
    {
        builder.Services.AddTransient(@interface, type);
    }
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseMiddleware<ProblemDetailsMiddleware>();
app.MapControllers();

try
{
    Log.Information("Starting User Management API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}