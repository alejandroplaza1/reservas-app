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
        [StringLength(100)]
        public string Email { get; set; }

        
        [Required]
        [StringLength(100)]
        public string Password { get; set; }
        

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
