using Microsoft.EntityFrameworkCore;
using ReservasApi.Data;
using ReservasApi.Models;
using System.Security.Cryptography; // Librería nativa para encriptar
using System.Text;

namespace ReservasApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        // Lógica para REGISTRAR un usuario
        public async Task<Usuario> RegistrarUsuarioAsync(string nombre, string email, string password)
        {
            // 1. Validar que el email no exista ya
            bool existe = await _context.Usuarios.AnyAsync(u => u.Email == email);
            if (existe)
            {
                throw new InvalidOperationException("El email ya está registrado en el sistema.");
            }

            // 2. Encriptar la contraseña antes de guardarla
            string passwordHash = EncriptarPassword(password);

            // 3. Crear el objeto Usuario
            var nuevoUsuario = new Usuario
            {
                Nombre = nombre,
                Email = email,
                Password = passwordHash, // Guardamos el Hash, NO el texto plano
                Rol = "Usuario" // Rol por defecto
            };

            // 4. Guardar en Base de Datos
            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return nuevoUsuario;
        }

        // Lógica para LOGIN
        public async Task<Usuario?> ValidarCredencialesAsync(string email, string password)
        {
            // 1. Buscar usuario por email
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null)
            {
                return null; // Usuario no existe
            }

            // 2. Verificar contraseña
            // Encriptamos la contraseña que nos acaban de dar...
            string passwordEntranteHash = EncriptarPassword(password);

            // ...y la comparamos con la que tenemos guardada en la BD
            if (usuario.Password != passwordEntranteHash)
            {
                return null; // Contraseña incorrecta
            }

            return usuario; // Todo OK
        }

        //funcion para encriptar la contraseña
        private string EncriptarPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // Convertimos el texto a bytes
                var bytes = Encoding.UTF8.GetBytes(password);
                // Calculamos el hash
                var hash = sha256.ComputeHash(bytes);
                // Lo convertimos a String para guardarlo en la BD
                return Convert.ToBase64String(hash);
            }
        }
    }
}
