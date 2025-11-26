using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasApi.Controllers;
using ReservasApi.Data;
using ReservasApi.Models;
using Xunit;
using System.Linq;

namespace ReservasApi.Tests.Controllers
{
    public class EspaciosControllerTests
    {
        private AppDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetEspacios_DeberiaRetornarListaDeEspacios()
        {
            // ARRANGE
            var dbContext = GetInMemoryDbContext("TestDb_Espacios_GetAll");

            // CORRECCIÓN: Añadimos 'Descripcion' porque es obligatorio
            dbContext.Espacios.Add(new Espacio { Nombre = "Sala A", Capacidad = 10, Descripcion = "Desc A" });
            dbContext.Espacios.Add(new Espacio { Nombre = "Sala B", Capacidad = 20, Descripcion = "Desc B" });

            await dbContext.SaveChangesAsync();

            var controller = new EspaciosController(dbContext);

            // ACT
            var actionResult = await controller.GetEspacios();

            // ASSERT
            var valorRetornado = actionResult.Value;
            Assert.NotNull(valorRetornado);
            Assert.Equal(2, valorRetornado!.Count());
        }

        [Fact]
        public async Task GetEspacio_DeberiaRetornarEspacio_CuandoIdExiste()
        {
            // ARRANGE
            var dbContext = GetInMemoryDbContext("TestDb_Espacios_GetOne");

            // CORRECCIÓN: Añadimos 'Descripcion'
            var espacioPrueba = new Espacio { Nombre = "Sala VIP", Capacidad = 5, Descripcion = "Desc VIP" };

            dbContext.Espacios.Add(espacioPrueba);
            await dbContext.SaveChangesAsync();

            int idGenerado = espacioPrueba.Id;

            var controller = new EspaciosController(dbContext);

            // ACT
            var actionResult = await controller.GetEspacio(idGenerado);

            // ASSERT
            var valorRetornado = actionResult.Value;
            Assert.NotNull(valorRetornado);
            Assert.Equal("Sala VIP", valorRetornado!.Nombre);
        }

        [Fact]
        public async Task GetEspacio_DeberiaRetornarNotFound_CuandoIdNoExiste()
        {
            // ARRANGE
            var dbContext = GetInMemoryDbContext("TestDb_Espacios_NotFound");
            var controller = new EspaciosController(dbContext);

            // ACT
            var actionResult = await controller.GetEspacio(999);

            // ASSERT
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }
    }
}
