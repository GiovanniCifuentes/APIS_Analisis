// Program.cs (ACTUALIZADO)
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using CRMVentasAPI;
using CRMVentasAPI.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// --- CONEXIÓN INTELIGENTE ---
string connectionString;
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MYSQLHOST")))
{
    var dbHost = Environment.GetEnvironmentVariable("MYSQLHOST");
    var dbPort = Environment.GetEnvironmentVariable("MYSQLPORT");
    var dbUser = Environment.GetEnvironmentVariable("MYSQLUSER");
    var dbPass = Environment.GetEnvironmentVariable("MYSQLPASSWORD");
    var dbName = Environment.GetEnvironmentVariable("MYSQLDATABASE");
    connectionString = $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={dbPass};";
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no fue encontrada.");
}

// Configurar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 🔹 NUEVO: Configurar HttpClient para API externa
builder.Services.AddHttpClient<IExternalApiService, ExternalApiService>();

// Configurar controladores
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CRM Ventas API - Integrado con Gestión de Contactos",
        Version = "v1",
        Description = "API completa de CRM integrada con el sistema externo de gestión de contactos"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Aplicar migraciones
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al aplicar las migraciones de la base de datos.");
    }
}

// Configurar pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // Esta línea le dice a Swagger dónde encontrar la definición de la API
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM Ventas API V1");
    // Esta línea hace que la página de Swagger sea la página de inicio de tu URL
    c.RoutePrefix = string.Empty;
});

app.UseCors("PermitirTodo");
app.UseAuthorization();
app.MapControllers();

app.Run();
