namespace GestionDeTareas.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public DateTime Expiracion { get; set; }
    }
}
