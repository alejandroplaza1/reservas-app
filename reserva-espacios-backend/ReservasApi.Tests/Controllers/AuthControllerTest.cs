using Microsoft.AspNetCore.Mvc;
using Moq;
using ReservasApi.Controllers;
using ReservasApi.Services;
using ReservasApi.Models;
using Xunit;

namespace ReservasApi.Tests.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_DeberiaRetornarOk_CuandoRegistroEsExitoso()
        {
            // 1. ARRANGE
            var mockService = new Mock<IAuthService>();

            // Configuramos el "doble" para que devuelva un usuario falso cuando lo llamen
            mockService.Setup(s => s.RegistrarUsuarioAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                       .ReturnsAsync(new Usuario { Id = 1, Nombre = "Juan", Email = "juan@test.com", Rol = "Usuario" });

            var controller = new AuthController(mockService.Object);
            var request = new RegisterRequest { Nombre = "Juan", Email = "juan@test.com", Password = "123" };

            // 2. ACT
            var resultado = await controller.Register(request);

            // 3. ASSERT
            var okResult = Assert.IsType<OkObjectResult>(resultado); // Verifica que sea un 200 OK
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Register_DeberiaRetornarConflict_CuandoEmailYaExiste()
        {
            // 1. ARRANGE
            var mockService = new Mock<IAuthService>();

            // Configuramos el "doble" para que lance error simulando un duplicado
            mockService.Setup(s => s.RegistrarUsuarioAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                       .ThrowsAsync(new InvalidOperationException("El email ya existe"));

            var controller = new AuthController(mockService.Object);
            var request = new RegisterRequest { Nombre = "Juan", Email = "duplicado@test.com", Password = "123" };

            // 2. ACT
            var resultado = await controller.Register(request);

            // 3. ASSERT
            var conflictResult = Assert.IsType<ConflictObjectResult>(resultado); // Verifica que sea un 409 Conflict
            Assert.Equal(409, conflictResult.StatusCode);
        }
    }
}