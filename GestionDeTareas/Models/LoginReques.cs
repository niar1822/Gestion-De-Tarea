using System.ComponentModel.DataAnnotations;

namespace GestionDeTareas.Models
{
    public class UserLoginRequest
    {
        [Required]
        public string NombreUsuario { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
