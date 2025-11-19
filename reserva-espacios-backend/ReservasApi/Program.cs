using Microsoft.EntityFrameworkCore;
using ReservasApi.Data; // <--- IMPORTANTE: Coincide con DbContext.cs

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar la conexión a Base de Datos
// (Asegúrate de que la ConnectionString "DefaultConnection" existe en tu appsettings.json)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Añadir controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Ejecutar el inicializador de datos (Seed Data)
// Usamos un scope para obtener el servicio de base de datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // Crea la BD si no existe
        context.Database.EnsureCreated();
        // Aquí puedes llamar a tu DbInitializer si lo tienes:
        // DbInitializer.Initialize(context); 
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar la base de datos.");
    }
}

// 4. Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Comentado a veces para evitar problemas con Docker local
app.UseAuthorization();

app.MapControllers();

app.Run();