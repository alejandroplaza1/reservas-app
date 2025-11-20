using ReservasApi.Models;

namespace ReservasApi.Services
{
    public interface IReservasService
    {
        Task<IEnumerable<Reserva>> ObtenerTodasAsync();
        Task<Reserva?> ObtenerPorIdAsync(int id);
        Task<Reserva> CrearReservaAsync(Reserva reserva);
        Task<bool> CancelarReservaAsync(int id);
        // Más adelante añadiremos Modificar y Finalizar
    }
}