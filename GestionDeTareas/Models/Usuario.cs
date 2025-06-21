using System.ComponentModel.DataAnnotations;

namespace GestionDeTareas.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string Rol { get; set; } = "Usuario";

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public bool Activo { get; set; } = true;
    
    }
}
