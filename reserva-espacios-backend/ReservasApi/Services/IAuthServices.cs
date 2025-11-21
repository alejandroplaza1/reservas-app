using ReservasApi.Models;

namespace ReservasApi.Services
{
    public interface IAuthService
    {
        // Método para registrar un nuevo usuario
        Task<Usuario> RegistrarUsuarioAsync(string nombre, string email, string password);

        // Método para validar el login (email y contraseña)
        Task<Usuario?> ValidarCredencialesAsync(string email, string password);
    }
}