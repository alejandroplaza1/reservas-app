using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReservasApi.Models
{
    public class Espacio
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(250)]
        public string Descripcion { get; set; }

        public int Capacidad { get; set; }

        public bool Disponible { get; set; } = true;

        // Navigation property
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
