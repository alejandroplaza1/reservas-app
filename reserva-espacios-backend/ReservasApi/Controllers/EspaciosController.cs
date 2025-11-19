using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasApi.Data;    // <--- IMPORTANTE: Usa el namespace corregido
using ReservasApi.Models;

namespace ReservasApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EspaciosController : ControllerBase
    {
        // Usamos la clase AppDbContext corregida
        private readonly AppDbContext _context;

        public EspaciosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Espacio>>> GetEspacios()
        {
            // Si no hay espacios, devolvemos lista vacía en lugar de error
            if (_context.Espacios == null)
            {
                return NotFound();
            }
            return await _context.Espacios.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Espacio>> GetEspacio(int id)
        {
            if (_context.Espacios == null)
            {
                return NotFound();
            }
            var espacio = await _context.Espacios.FindAsync(id);

            if (espacio == null)
            {
                return NotFound();
            }

            return espacio;
        }
    }
}
