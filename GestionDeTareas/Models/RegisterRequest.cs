using System.ComponentModel.DataAnnotations;

namespace GestionDeTareas.Models
{
    public class UserRegisterRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string ConfirmarPassword { get; set; } = string.Empty;
    }
}
