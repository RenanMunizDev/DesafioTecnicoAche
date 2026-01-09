using DesafioTecnico_Ache.Interfaces;
using DesafioTecnico_Ache.Repositories;
using DesafioTecnico_Ache.Services;
using DesafioTecnico_Ache.Middlewares;
using DesafioTecnico_Ache.Validators;
using FluentValidation;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog para logging estruturado
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "SAP-Integration-API")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// Dependency Injection - Repository Pattern
builder.Services.AddSingleton<ISalesOrderRepository, SapSalesOrderRepository>();

// Dependency Injection - Service Layer Pattern
builder.Services.AddScoped<ISalesOrderService, SalesOrderService>();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateSalesOrderRequestValidator>();

// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SAP S/4HANA Sales Order Integration API",
        Version = "v1",
        Description = @"API REST para integração com SAP S/4HANA - Módulo SD (Sales & Distribution)

**Tipo de Integração:** OData v4 / REST API

**Módulo SAP:** SD (Sales & Distribution) - Gestão de Pedidos

**Dados Mockados:**
- Clientes: C001 a C005 (principais redes de farmácias brasileiras)
- Materiais: M001 a M005 (medicamentos comuns)
- Esta é uma implementação de demonstração com dados simulados",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Aché Laboratórios Farmacêuticos",
            Email = "integracao@ache.com.br"
        }
    });

    // Adicionar suporte para XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Adicionar esquema de segurança API Key
    options.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "API Key necessária para autenticação. Header: X-API-Key",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Name = "X-API-Key",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SAP Integration API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "SAP S/4HANA Integration API";
    });
}

// Middlewares customizados (ordem é importante!)
// 1. Exception Handling (primeiro para capturar todos os erros)
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// 2. Rate Limiting (antes da autenticação)
app.UseMiddleware<RateLimitingMiddleware>();

// 3. API Key Authentication
app.UseMiddleware<ApiKeyAuthenticationMiddleware>();
// Middlewares padrão ASP.NET Core
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Log de inicialização
Log.Information("SAP S/4HANA Sales Order Integration API iniciada");
Log.Information("Ambiente: {Environment}", app.Environment.EnvironmentName);

// Obter URLs configuradas
var urls = builder.Configuration["ASPNETCORE_URLS"]?.Split(';') ?? 
           builder.WebHost.GetSetting("urls")?.Split(';') ?? 
           new[] { "http://localhost:5000" };

Log.Information("========================================");
Log.Information("URLs da API:");
foreach (var url in urls)
{
    Log.Information("  - {Url}", url);
}

if (app.Environment.IsDevelopment())
{
    Log.Information("========================================");
    Log.Information("Swagger disponível em:");
    foreach (var url in urls)
    {
        Log.Information("  - {SwaggerUrl}", $"{url}/swagger");
    }
}
Log.Information("========================================");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação falhou ao iniciar");
}
finally
{
    Log.CloseAndFlush();
}
