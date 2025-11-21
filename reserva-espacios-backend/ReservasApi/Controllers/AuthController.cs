using Microsoft.AspNetCore.Mvc;
using ReservasApi.Services; // Asegúrate de tener este using

namespace ReservasApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ---------------------------------------------------------
        // 1. AQUÍ ESTÁ EL REGISTRO (HttpPost)
        // ---------------------------------------------------------
        [HttpPost("register")] // <--- ESTA ES LA ETIQUETA QUE PREGUNTAS
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var usuario = await _authService.RegistrarUsuarioAsync(request.Nombre, request.Email, request.Password);
                return Ok(new { mensaje = "Usuario registrado exitosamente", id = usuario.Id });
            }
            catch (InvalidOperationException ex)
            {
                // Devuelve error 409 si el email ya existe
                return Conflict(new { mensaje = ex.Message });
            }
        }

        // ---------------------------------------------------------
        // 2. AQUÍ ESTÁ EL LOGIN (HttpPost)
        // ---------------------------------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var usuario = await _authService.ValidarCredencialesAsync(request.Email, request.Password);

            if (usuario == null)
            {
                return Unauthorized(new { mensaje = "Email o contraseña incorrectos." });
            }

            return Ok(new
            {
                mensaje = "Login exitoso",
                usuarioId = usuario.Id,
                nombre = usuario.Nombre,
                rol = usuario.Rol
            });
        }
    }

    // ---------------------------------------------------------
    // 3. LAS CLASES PARA RECIBIR DATOS (DTOs)
    // ---------------------------------------------------------
    public class RegisterRequest
    {
        public required string Nombre { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}