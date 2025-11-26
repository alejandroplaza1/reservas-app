using Microsoft.AspNetCore.Mvc;
using Moq;
using ReservasApi.Controllers;
using ReservasApi.Models;
using ReservasApi.Services;
using Xunit;

namespace ReservasApi.Tests.Controllers
{
    public class ReservasControllerTests
    {
        [Fact]
        public async Task GetReservas_DeberiaRetornarLista_YStatus200()
        {
            // 1. ARRANGE
            var mockService = new Mock<IReservasService>();

            // Simulamos que el servicio devuelve una lista vacía
            mockService.Setup(s => s.ObtenerTodasAsync())
                       .ReturnsAsync(new List<Reserva>());

            var controller = new ReservasController(mockService.Object);

            // 2. ACT
            var resultado = await controller.GetReservas();

            // 3. ASSERT
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task PostReserva_DeberiaRetornarCreated_CuandoEsValida()
        {
            // 1. ARRANGE
            var mockService = new Mock<IReservasService>();
            var reservaInput = new Reserva { EspacioId = 1, UsuarioId = 1 };
            var reservaCreada = new Reserva { Id = 100, EspacioId = 1, UsuarioId = 1 };

            // Configuramos el mock para que devuelva la reserva creada con ID
            mockService.Setup(s => s.CrearReservaAsync(It.IsAny<Reserva>()))
                       .ReturnsAsync(reservaCreada);

            var controller = new ReservasController(mockService.Object);

            // 2. ACT
            var resultado = await controller.PostReserva(reservaInput);

            // 3. ASSERT
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            Assert.Equal(201, createdResult.StatusCode); // 201 Created
            Assert.Equal(100, ((Reserva)createdResult.Value!).Id); // Verificamos que devuelve el ID correcto
        }

        [Fact]
        public async Task PostReserva_DeberiaRetornarBadRequest_CuandoFallaValidacion()
        {
            // 1. ARRANGE
            var mockService = new Mock<IReservasService>();

            // Simulamos que el servicio lanza ArgumentException (ej. hora incorrecta)
            mockService.Setup(s => s.CrearReservaAsync(It.IsAny<Reserva>()))
                       .ThrowsAsync(new ArgumentException("Error de validación"));

            var controller = new ReservasController(mockService.Object);
            var reserva = new Reserva();

            // 2. ACT
            var resultado = await controller.PostReserva(reserva);

            // 3. ASSERT
            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task PostReserva_DeberiaRetornarConflict_CuandoHaySolapamiento()
        {
            // 1. ARRANGE
            var mockService = new Mock<IReservasService>();

            // Simulamos que el servicio lanza InvalidOperationException (ej. ya ocupado)
            mockService.Setup(s => s.CrearReservaAsync(It.IsAny<Reserva>()))
                       .ThrowsAsync(new InvalidOperationException("Ya reservado"));

            var controller = new ReservasController(mockService.Object);
            var reserva = new Reserva();

            // 2. ACT
            var resultado = await controller.PostReserva(reserva);

            // 3. ASSERT
            var conflict = Assert.IsType<ConflictObjectResult>(resultado.Result);
            Assert.Equal(409, conflict.StatusCode);
        }
    }
}