using System.ComponentModel;
using System.Numerics;
using System.Text;
using GestionDeTareas.Helpers;
using GestionDeTareas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaksModels.Models;
using static GestionDeTareas.Factorymetho.GestionDeTareas;
using System.Security.Cryptography;
using System.Security.Claims;

namespace GestionDeTareas.Controllers
{

    [Authorize] 
    [ApiController]
    [Route("api/[controller]")]
    public class Taskcontroller : ControllerBase
    {
        private readonly TaskContext _Context;
        private readonly TaskQueue _taskQueue;
        private readonly ITaskNotificationService _notificationService;

        public Taskcontroller(TaskContext context, TaskQueue taskQueue, ITaskNotificationService notificationService)
        {
            _Context = context;
            _taskQueue = taskQueue;
            _notificationService = notificationService;
        }

        [HttpPost]
        [Route("crear")]
        public async Task<IActionResult> Creartarea(CrearDataRequest request)
        {
            var tareaGenerica = TareaFactory.Crear(
                request.Tipo,
                request.Titulo!,
                request.Description!,
                request.DueDate,
                request.Status!,
                request.ExtraData!
            );

            var tarea = new TaskData
            {
                Titulo = tareaGenerica.Titulo,
                Description = tareaGenerica.Description,
                DueDate = tareaGenerica.DueDate,
                Status = tareaGenerica.Status,
                ExtraData = tareaGenerica.ExtraData.ToString()
            };

            ValidarCampo validarCampo = Validaciones.ValidarDescripcion;
            validarCampo += Validaciones.ValidarFecha;

            Action<TaskData> accionespost = Acciones.NotificarCreacion;

            foreach (ValidarCampo item in validarCampo.GetInvocationList())
            {
                if (!item(tarea))
                {
                    return BadRequest("Error en la validación de los campos");
                }
            }

            await _Context.Tasks.AddAsync(tarea);
            await _Context.SaveChangesAsync();
            MemoService<string>.LimpiarCache();

            _taskQueue.Enqueue(tareaGenerica);
            accionespost(tarea);

            // Obtener info del usuario autenticado (solo ejemplo)
            var nombreUsuario = User.Identity?.Name;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            await _notificationService.NotifyTaskCreated(new
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                Description = tarea.Description,
                Status = tarea.Status,
                DueDate = tarea.DueDate,
                Mensaje = $"Nueva tarea creada por {email}"
            });

            return Ok(new
            {
                tarea.Titulo,
                tarea.Description,
                tarea.Status,
                tarea.Tipo,
                mensaje = "Tarea creada exitosamente",
                diasrestantes = Calculardias.CalcularDiasRestantes(tarea)
            });
        }

        [HttpGet]
        [Route("listar")]
        public async Task<ActionResult<IEnumerable<TaskData>>> Listartareas()
        {
            var tareas = await _Context.Tasks.ToListAsync();
            return Ok(tareas);
        }

        [HttpGet]
        [Route("ver")]
        public async Task<IActionResult> VerTarea(int id)
        {
            var tarea = await _Context.Tasks.FindAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }
            return Ok(tarea);
        }

        [HttpGet]
        [Route("obtenertareascompletadas")]
        public async Task<IActionResult> ObtenerPorcentajeTareasCompletadas()
        {
            var tareas = await _Context.Tasks.ToListAsync();
            double porcentaje = MemoService<string>.ObtenerPorcentajeCompletadas(tareas);
            return Ok(new { porcentaje });
        }

        [HttpGet]
        [Route("filtrarporestado")]
        public async Task<IActionResult> FiltrarTareasPorEstado([FromQuery] string estado)
        {
            var tareas = await _Context.Tasks.ToListAsync();
            var filtradas = MemoService<string>.FiltrarPorEstado(tareas, estado);
            return Ok(filtradas);
        }

        [HttpPut]
        [Route("editar")]
        public async Task<IActionResult> EditarTarea(int id, TaskData tarea)
        {
            var TareaExistente = await _Context.Tasks.FindAsync(id);
            if (TareaExistente == null)
            {
                return NotFound();
            }

            TareaExistente.Description = tarea.Description;
            TareaExistente.DueDate = tarea.DueDate;
            TareaExistente.Status = tarea.Status;
            TareaExistente.ExtraData = tarea.ExtraData;

            await _Context.SaveChangesAsync();

            await _notificationService.NotifyTaskUpdated(new
            {
                Id = TareaExistente.Id,
                Titulo = TareaExistente.Titulo,
                Description = TareaExistente.Description,
                Status = TareaExistente.Status,
                DueDate = TareaExistente.DueDate,
                Mensaje = "Tarea actualizada"
            });

            return Ok();
        }
        [HttpDelete]
        [Route("eliminar")]
        public async Task<IActionResult> EliminarTarea(int id)
        {
            var tarea = await _Context.Tasks.FindAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }

            _Context.Tasks.Remove(tarea);
            await _Context.SaveChangesAsync();

            await _notificationService.NotifyTaskDeleted(id);
            return Ok();
        }
    }

}

// ========== MOVER AUTHCONTROLLER FUERA ==========
[ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
            private readonly TaskContext _context;
            private readonly IJwtService _jwtService;
            private readonly ILogger<AuthController> _logger;

            public AuthController(TaskContext context, IJwtService jwtService, ILogger<AuthController> logger)
            {
                _context = context;
                _jwtService = jwtService;
                _logger = logger;
            }

            [HttpPost("login")]
            public async Task<ActionResult<LoginResponse>> Login([FromBody] UserLoginRequest request)
            {
                try
                {

                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    // Buscar usuario por nombre de usuario
                    var usuario = await _context.Usuarios
                        .FirstOrDefaultAsync(u => u.NombreUsuario == request.NombreUsuario && u.Activo);

                    if (usuario == null)
                    {
                        return Unauthorized(new { mensaje = "Credenciales inválidas" });
                    }

                    // Verificar contraseña
                    if (!VerificarPassword(request.Password, usuario.PasswordHash))
                    {
                        return Unauthorized(new { mensaje = "Credenciales inválidas" });
                    }

                    // Generar token JWT
                    var token = _jwtService.GenerarToken(usuario);
                    var expiracion = DateTime.UtcNow.AddHours(24);

                    var response = new LoginResponse
                    {
                        Token = token,
                        NombreUsuario = usuario.NombreUsuario,
                        Email = usuario.Email,
                        Rol = usuario.Rol,
                        Expiracion = expiracion
                    };

                    _logger.LogInformation($"Usuario {usuario.NombreUsuario} ha iniciado sesión exitosamente");

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error durante el login");
                    return StatusCode(500, new { mensaje = "Error interno del servidor" });
                }
            }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> Register([FromBody] UserRegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar si el usuario ya existe
            var usuarioExistente = await _context.Usuarios
                .AnyAsync(u => u.NombreUsuario == request.NombreUsuario || u.Email == request.Email);

            if (usuarioExistente)
            {
                return Conflict(new { mensaje = "El usuario o email ya existe" });
            }

            // Crear nuevo usuario
            var nuevoUsuario = new Usuario
            {
                NombreUsuario = request.NombreUsuario,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Rol = "Usuario",
                FechaCreacion = DateTime.UtcNow,
                Activo = true
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // Generar token JWT
            var token = _jwtService.GenerarToken(nuevoUsuario);
            var expiracion = DateTime.UtcNow.AddHours(24);

            var response = new LoginResponse
            {
                Token = token,
                NombreUsuario = nuevoUsuario.NombreUsuario,
                Email = nuevoUsuario.Email,
                Rol = nuevoUsuario.Rol,
                Expiracion = expiracion
            };

            _logger.LogInformation($"Nuevo usuario {nuevoUsuario.NombreUsuario} registrado exitosamente");

            return CreatedAtAction(nameof(Login), response);
        }
       

            [HttpPost("validar-token")]
            public IActionResult ValidarToken([FromBody] string token)
            {
                try
                {
                    if (string.IsNullOrEmpty(token))
                    {
                        return BadRequest(new { mensaje = "Token requerido" });
                    }

                    // Remover "Bearer " si está presente
                    if (token.StartsWith("Bearer "))
                    {
                        token = token.Substring(7);
                    }

                    var esValido = _jwtService.ValidarToken(token);

                    if (esValido)
                    {
                        var usuario = _jwtService.ObtenerUsuarioDelToken(token);
                        var rol = _jwtService.ObtenerRolDelToken(token);

                        return Ok(new
                        {
                            valido = true,
                            usuario = usuario,
                            rol = rol
                        });
                    }

                    return Unauthorized(new { valido = false, mensaje = "Token inválido o expirado" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al validar token");
                    return StatusCode(500, new { mensaje = "Error interno del servidor" });
                }
            }

            private string HashPassword(string password)
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return Convert.ToBase64String(hashedBytes);
                }
            }

            private bool VerificarPassword(string password, string hash)
            {
                var passwordHash = HashPassword(password);
                return passwordHash == hash;
            }
        }