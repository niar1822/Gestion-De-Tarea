using System.Security.Claims;

namespace GestionDeTareas.Models
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtService _jwtService;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, IJwtService jwtService, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = ExtraerToken(context);

            if (!string.IsNullOrEmpty(token))
            {
                await ValidarYAsignarUsuario(context, token);
            }

            await _next(context);
        }

        private string? ExtraerToken(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader))
                return null;

            if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }

            return null;
        }

        private async Task ValidarYAsignarUsuario(HttpContext context, string token)
        {
            try
            {
                if (_jwtService.ValidarToken(token))
                {
                    var nombreUsuario = _jwtService.ObtenerUsuarioDelToken(token);
                    var rol = _jwtService.ObtenerRolDelToken(token);

                    if (!string.IsNullOrEmpty(nombreUsuario))
                    {
                        // Crear claims para el usuario autenticado
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, nombreUsuario),
                            new Claim(ClaimTypes.Role, rol ?? "Usuario")
                        };

                        var identity = new ClaimsIdentity(claims, "jwt");
                        context.User = new ClaimsPrincipal(identity);

                        _logger.LogDebug($"Usuario {nombreUsuario} autenticado correctamente");
                    }
                }
                else
                {
                    _logger.LogWarning("Token JWT inválido o expirado");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar token JWT en middleware");
            }
        }
    }
    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtMiddleware>();
        }
    }
}
