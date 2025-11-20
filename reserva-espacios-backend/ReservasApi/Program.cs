using Microsoft.EntityFrameworkCore;
using ReservasApi.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar la conexión a Base de Datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- NUEVO: Configurar CORS ---
// Definimos una política llamada "AllowAngular" (o el nombre que prefieras)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // La URL de tu Frontend en el navegador
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// -----------------------------

// 2. Añadir controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Ejecutar el inicializador de datos (Seed Data)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
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

// app.UseHttpsRedirection();

//activar el cors
app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();