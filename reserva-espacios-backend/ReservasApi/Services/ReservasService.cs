using Microsoft.EntityFrameworkCore;
using ReservasApi.Data;
using ReservasApi.Models;

namespace ReservasApi.Services
{
    public class ReservasService : IReservasService
    {
        private readonly AppDbContext _context;

        public ReservasService(AppDbContext context)
        {
            _context = context;
        }

        // Obtener todas las reservas (incluyendo datos de Usuario y Espacio para el frontend)
        public async Task<IEnumerable<Reserva>> ObtenerTodasAsync()
        {
            return await _context.Reservas
                .Include(r => r.Espacio)
                .Include(r => r.Usuario)
                .ToListAsync();
        }

        // Obtener una reserva específica por ID
        public async Task<Reserva?> ObtenerPorIdAsync(int id)
        {
            return await _context.Reservas
                .Include(r => r.Espacio)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Lógica principal de creación con validaciones estrictas
        public async Task<Reserva> CrearReservaAsync(Reserva reserva)
        {
            //1. VALIDACIONES DE REGLAS DE NEGOCIO

            // 1.1. Validar que la reserva sea en punto (ej. 10:00:00)
            if (reserva.FechaInicio.Minute != 0 || reserva.FechaInicio.Second != 0)
            {
                throw new ArgumentException("Las reservas deben comenzar a la hora en punto (ej. 10:00, no 10:30).");
            }

            // 1.2. Validar duración exacta de 1 hora
            if (reserva.FechaFin != reserva.FechaInicio.AddHours(1))
            {
                throw new ArgumentException("La duración de la reserva debe ser de exactamente 1 hora.");
            }

            // 1.3. Validar rango horario permitido (08:00 a 22:00)
            // La pista abre a las 8 y cierra a las 22.
            // La última reserva posible es de 21:00 a 22:00.
            // Por tanto, la hora de inicio debe ser >= 8 y < 22.
            int horaInicio = reserva.FechaInicio.Hour;

            if (horaInicio < 8 || horaInicio >= 22)
            {
                throw new ArgumentException("El horario de las pistas es exclusivamente de 08:00 a 22:00.");
            }

            // 1.4. No permitir reservas en el pasado
            if (reserva.FechaInicio < DateTime.Now)
            {
                throw new ArgumentException("No puedes realizar reservas en el pasado.");
            }

            //VALIDACIÓN DE SOLAPAMIENTO (Disponibilidad)

            // Buscamos si ya existe una reserva activa (no cancelada) en ese espacio y horario
            bool ocupado = await _context.Reservas.AnyAsync(r =>
                r.EspacioId == reserva.EspacioId &&
                r.Estado != EstadoReserva.Cancelada &&
                ((reserva.FechaInicio >= r.FechaInicio && reserva.FechaInicio < r.FechaFin) ||
                 (reserva.FechaFin > r.FechaInicio && reserva.FechaFin <= r.FechaFin) ||
                 (reserva.FechaInicio <= r.FechaInicio && reserva.FechaFin >= r.FechaFin)));

            if (ocupado)
            {
                throw new InvalidOperationException("El espacio ya está reservado en ese horario.");
            }

            // --- 3. GUARDADO EN BASE DE DATOS ---

            reserva.Estado = EstadoReserva.Confirmada; // Se confirma automáticamente al pasar validaciones
            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            return reserva;
        }

        // Cancelar una reserva (Cambio de estado)
        public async Task<bool> CancelarReservaAsync(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return false;

            reserva.Estado = EstadoReserva.Cancelada;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
