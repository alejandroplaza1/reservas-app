using ReservasApi.Data;
using ReservasApi.Models;

namespace ReservasApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // 1. Crear Pista si no existe
            if (!context.Espacios.Any())
            {
                context.Espacios.Add(new Espacio
                {
                    Nombre = "Pista Central",
                    Capacidad = 4,
                    Descripcion = "Pista de cristal oficial",
                    Disponible = true
                });
                context.SaveChanges(); // Guardamos para generar el ID
            }

            // 2. Crear Usuario de Prueba si no existe (NUEVO)
            if (!context.Usuarios.Any())
            {
                context.Usuarios.Add(new Usuario
                {
                    Nombre = "Jugador Demo",
                    Email = "demo@test.com",
                    Password = "123456"
                });
                context.SaveChanges();
            }
        }
    }
}
