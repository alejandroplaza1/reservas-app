using Microsoft.AspNetCore.Mvc;
using ReservasApi.Models;
using ReservasApi.Services; // <--- Importante

namespace ReservasApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        // En lugar de AppDbContext, ahora inyectamos la interfaz del servicio
        private readonly IReservasService _reservasService;

        public ReservasController(IReservasService reservasService)
        {
            _reservasService = reservasService;
        }

        // GET: api/Reservas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReservas()
        {
            var reservas = await _reservasService.ObtenerTodasAsync();
            return Ok(reservas);
        }

        // POST: api/Reservas
        [HttpPost]
        public async Task<ActionResult<Reserva>> PostReserva(Reserva reserva)
        {
            try
            {
                // Delegamos toda la validación y guardado al servicio
                var nuevaReserva = await _reservasService.CrearReservaAsync(reserva);

                return CreatedAtAction("GetReservas", new { id = nuevaReserva.Id }, nuevaReserva);
            }
            catch (ArgumentException ex) // Errores de validación (hora mal, pasado, etc.)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex) // Errores de conflicto (ya ocupado)
            {
                return Conflict(new { mensaje = ex.Message });
            }
            catch (Exception ex) // Cualquier otro error inesperado
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error interno.", detalle = ex.Message });
            }
        }
    }
}