namespace GestionDeTareas.Models
{
    public interface IJwtService
    {
        string GenerarToken(Usuario usuario);
        bool ValidarToken(string token);
        string? ObtenerUsuarioDelToken(string token);
        string? ObtenerRolDelToken(string token);
    }
}
