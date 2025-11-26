using Microsoft.EntityFrameworkCore;
using ReservasApi.Data;
using ReservasApi.Models;
using ReservasApi.Services;
using Xunit;

namespace ReservasApi.Tests.Services
{
    public class ReservasServiceTests
    {
        private AppDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CrearReserva_DeberiaFuncionar_CuandoTodoEsCorrecto()
        {
            // ARRANGE
            var dbContext = GetInMemoryDbContext("TestDb_Reserva_Exito");
            var service = new ReservasService(dbContext);

            // Creamos una fecha futura válida (ej. mañana a las 10:00)
            var fechaValida = DateTime.Today.AddDays(1).AddHours(10);

            var reserva = new Reserva
            {
                EspacioId = 1,
                UsuarioId = 1,
                FechaInicio = fechaValida,
                FechaFin = fechaValida.AddHours(1) // Duración exacta de 1h
            };

            // ACT
            var resultado = await service.CrearReservaAsync(reserva);

            // ASSERT
            Assert.NotNull(resultado);
            Assert.Equal(EstadoReserva.Confirmada, resultado.Estado);
        }

        [Fact]
        public async Task CrearReserva_DeberiaFallar_SiNoEsHoraEnPunto()
        {
            // ARRANGE
            var dbContext = GetInMemoryDbContext("TestDb_Reserva_Minutos");
            var service = new ReservasService(dbContext);

            // Intentamos reservar a las 10:30 (No permitido)
            var fechaMala = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);

            var reserva = new Reserva
            {
                EspacioId = 1,
                FechaInicio = fechaMala,
                FechaFin = fechaMala.AddHours(1)
            };

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearReservaAsync(reserva));
            Assert.Equal("Las reservas deben comenzar a la hora en punto (ej. 10:00, no 10:30).", ex.Message);
        }

        [Fact]
        public async Task CrearReserva_DeberiaFallar_SiDuracionNoEsUnaHora()
        {
            // ARRANGE
            var dbContext = GetInMemoryDbContext("TestDb_Reserva_Duracion");
            var service = new ReservasService(dbContext);

            var fecha = DateTime.Today.AddDays(1).AddHours(10);

            var reserva = new Reserva
            {
                EspacioId = 1,
                FechaInicio = fecha,
                FechaFin = fecha.AddHours(2) // <-- 2 HORAS (No permitido)
            };

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearReservaAsync(reserva));
            Assert.Equal("La duración de la reserva debe ser de exactamente 1 hora.", ex.Message);
        }

        [Fact]
        public async Task CrearReserva_DeberiaFallar_SiEsFueraDeHorario()
        {
            // ARRANGE
            var dbContext = GetInMemoryDbContext("TestDb_Reserva_Cierre");
            var service = new ReservasService(dbContext);

            // Intentamos reservar a las 23:00 (Cierra a las 22:00)
            var fechaNoche = DateTime.Today.AddDays(1).AddHours(23);

            var reserva = new Reserva
            {
                EspacioId = 1,
                FechaInicio = fechaNoche,
                FechaFin = fechaNoche.AddHours(1)
            };

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearReservaAsync(reserva));
            Assert.Contains("El horario de las pistas es exclusivamente de 08:00 a 22:00", ex.Message);
        }

        [Fact]
        public async Task CrearReserva_DeberiaFallar_SiYaEstaOcupado()
        {
            // ARRANGE
            var dbContext = GetInMemoryDbContext("TestDb_Reserva_Solapada");
            var service = new ReservasService(dbContext);
            var fecha = DateTime.Today.AddDays(1).AddHours(10);

            // 1. Insertamos una reserva existente
            dbContext.Reservas.Add(new Reserva
            {
                EspacioId = 1,
                FechaInicio = fecha,
                FechaFin = fecha.AddHours(1),
                Estado = EstadoReserva.Confirmada
            });
            await dbContext.SaveChangesAsync();

            // 2. Intentamos crear OTRA reserva igual
            var nuevaReserva = new Reserva
            {
                EspacioId = 1,
                FechaInicio = fecha,
                FechaFin = fecha.AddHours(1)
            };

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CrearReservaAsync(nuevaReserva));
            Assert.Equal("El espacio ya está reservado en ese horario.", ex.Message);
        }
    }
}
