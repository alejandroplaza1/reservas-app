using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasApi.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int EspacioId { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required]
        public EstadoReserva Estado { get; set; } = EstadoReserva.Pendiente;

        // Navigation properties
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [ForeignKey("EspacioId")]
        public Espacio Espacio { get; set; }
    }

    public enum EstadoReserva
    {
        Pendiente,
        Confirmada,
        Cancelada
    }
}
