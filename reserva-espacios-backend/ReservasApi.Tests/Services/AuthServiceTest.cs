using Microsoft.EntityFrameworkCore;
using ReservasApi.Data;
using ReservasApi.Services;
using ReservasApi.Models;
using Xunit;

namespace ReservasApi.Tests.Services
{
    public class AuthServiceTests
    {
        // Método auxiliar para crear una "base de datos virtual" limpia para cada test
        private AppDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task RegistrarUsuarioAsync_DeberiaGuardarUsuario_CuandoEmailEsNuevo()
        {
            // 1. ARRANGE (Preparar)
            var dbContext = GetInMemoryDbContext("TestDb_Registro_Exito");
            var authService = new AuthService(dbContext);

            string nombre = "Test User";
            string email = "nuevo@test.com";
            string password = "Password123!";

            // 2. ACT (Actuar)
            var resultado = await authService.RegistrarUsuarioAsync(nombre, email, password);

            // 3. ASSERT (Verificar)
            Assert.NotNull(resultado); // Que devuelva algo
            Assert.Equal(email, resultado.Email); // Que el email sea el correcto
            Assert.NotEqual(password, resultado.Password); // ¡IMPORTANTE! Que la contraseña NO sea texto plano
            Assert.Equal("Usuario", resultado.Rol); // Que tenga el rol por defecto
        }

        [Fact]
        public async Task RegistrarUsuarioAsync_DeberiaLanzarExcepcion_CuandoEmailYaExiste()
        {
            // 1. ARRANGE
            var dbContext = GetInMemoryDbContext("TestDb_Registro_Duplicado");
            var authService = new AuthService(dbContext);

            // Simulamos que ya existe un usuario en la BD
            dbContext.Usuarios.Add(new Usuario
            {
                Nombre = "Existente",
                Email = "duplicado@test.com",
                Password = "hash",
                Rol = "Usuario"
            });
            await dbContext.SaveChangesAsync();

            // 2. ACT & ASSERT
            // Esperamos que lance un error específico al intentar registrar el mismo email
            var excepcion = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                authService.RegistrarUsuarioAsync("Nuevo", "duplicado@test.com", "123456"));

            Assert.Equal("El email ya está registrado en el sistema.", excepcion.Message);
        }
    }
}
