using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasApi.Data;
using ReservasApi.Models;

namespace ReservasApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Reservas
        // Para que el calendario del Frontend sepa qué horas pintar en ROJO (ocupadas)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReservas()
        {
            return await _context.Reservas.ToListAsync();
        }

        // POST: api/Reservas
        // Aquí es donde se crea la reserva (La funcionalidad principal)
        [HttpPost]
        public async Task<ActionResult<Reserva>> PostReserva(Reserva reserva)
        {
            // 1. VALIDACIÓN CLAVE: ¿La pista ya está ocupada en esas horas?
            // Buscamos si existe alguna reserva para ESE espacio que solape con las horas pedidas
            bool ocupado = await _context.Reservas.AnyAsync(r =>
                r.EspacioId == reserva.EspacioId &&
                r.Estado != EstadoReserva.Cancelada && // Ignoramos las canceladas
                ((reserva.FechaInicio >= r.FechaInicio && reserva.FechaInicio < r.FechaFin) ||
                 (reserva.FechaFin > r.FechaInicio && reserva.FechaFin <= r.FechaFin) ||
                 (reserva.FechaInicio <= r.FechaInicio && reserva.FechaFin >= r.FechaFin)));

            if (ocupado)
            {
                return BadRequest("¡Lo siento! Ese horario ya está reservado.");
            }

            // 2. Si está libre, guardamos la reserva
            reserva.Estado = EstadoReserva.Confirmada; // La confirmamos directamente por ahora
            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReservas", new { id = reserva.Id }, reserva);
        }
    }
}