using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using CRMVentasAPI;
using CRMVentasAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// --- INICIO: LÓGICA DE CONEXIÓN INTELIGENTE ---
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
// --- FIN: LÓGICA DE CONEXIÓN INTELIGENTE ---

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CRM Ventas API", Version = "v1" });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

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

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM Ventas API V1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("PermitirTodo");
app.UseAuthorization();
app.MapControllers();
app.Run();
