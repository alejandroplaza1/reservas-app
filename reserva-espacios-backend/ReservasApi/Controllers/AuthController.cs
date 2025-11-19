using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasApi.Data; // Ajusta esto según tu namespace real en DbContext
using ReservasApi.Models;

namespace ReservasApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Buscar el usuario por Email
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            // 2. Verificar si existe y si la contraseña coincide
            // (Nota: En producción usaríamos hashes, aquí comparamos texto plano para la prueba)
            if (usuario == null || usuario.Password != request.Password)
            {
                return Unauthorized("Email o contraseña incorrectos.");
            }

            // 3. Si todo es correcto, devolvemos éxito y los datos del usuario
            return Ok(new
            {
                mensaje = "Login exitoso",
                usuarioId = usuario.Id,
                nombre = usuario.Nombre
            });
        }
    }

    // Clase auxiliar para recibir los datos del JSON
    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
