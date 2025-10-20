using CRMVentasAPI.Data; // Puede que necesites cambiar "Data" por el namespace correcto de tu AppDbContext
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔹 LEER VARIABLES DE ENTORNO DE RAILWAY PARA CONECTAR A MYSQL
// Railway inyecta estas variables automáticamente en tu servicio.
var dbHost = Environment.GetEnvironmentVariable("MYSQLHOST") ?? "mysql.railway.internal";
var dbPort = Environment.GetEnvironmentVariable("MYSQLPORT") ?? "3306";
var dbUser = Environment.GetEnvironmentVariable("MYSQLUSER") ?? "root";
var dbPass = Environment.GetEnvironmentVariable("MYSQLPASSWORD") ?? "ffKsHZolSMmAkvWHQBdItwClNVeTyrEi";
var dbName = Environment.GetEnvironmentVariable("MYSQLDATABASE") ?? "railway";

// 🔹 CONSTRUIR LA CADENA DE CONEXIÓN DINÁMICAMENTE
var connectionString = $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={dbPass};";

// 🔹 Configurar DbContext con la cadena de conexión de Railway
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 🔹 Configurar controladores y Swagger (esto se queda igual)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CRM Ventas API",
        Version = "v1",
        Description = "API para gestión de embudos de ventas",
        Contact = new OpenApiContact
        {
            Name = "Equipo de Desarrollo",
            Email = "soporte@crmventas.com"
        }
    });
});

// 🔹 Configurar CORS (esto se queda igual)
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 🔹 EJECUTAR MIGRACIONES DE LA BASE DE DATOS AUTOMÁTICAMENTE AL INICIAR
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        // Aplica cualquier migración pendiente a la base de datos.
        // Esto creará las tablas si no existen.
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al aplicar las migraciones de la base de datos.");
    }
}

// 🔹 Activar Swagger (esto puede quedarse, no afecta en producción)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM Ventas API V1");
    // 🔹 Para que Swagger esté disponible en la raíz en producción
    c.RoutePrefix = string.Empty;
});


// 🔹 Middleware (esto se queda igual)
app.UseCors("PermitirTodo");
app.UseAuthorization();

// 🔹 Mapear controladores
app.MapControllers();

// 🔹 Ejecutar la aplicación
app.Run();
