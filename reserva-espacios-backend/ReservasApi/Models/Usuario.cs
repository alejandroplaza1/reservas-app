using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReservasApi.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public RolUsuario Rol { get; set; } = RolUsuario.Usuario;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }

    public enum RolUsuario
    {
        Admin,
        Usuario
    }
}
