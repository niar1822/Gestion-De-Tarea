using System.ComponentModel;
using System.Numerics;
using GestionDeTareas.Helpers;
using GestionDeTareas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaksModels.Models;
using static GestionDeTareas.Factorymetho.GestionDeTareas;

namespace GestionDeTareas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Taskcontroller : ControllerBase
    {
        private readonly TaskContext _Context;
        private readonly TaskQueue _taskQueue;
        private readonly ITaskNotificationService _notificationService; // ← AGREGAR

        public Taskcontroller(TaskContext context, TaskQueue taskQueue, ITaskNotificationService notificationService) // ← MODIFICAR
        {
            _Context = context;
            _taskQueue = taskQueue;
            _notificationService = notificationService; // ← AGREGAR
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

            // ========== AGREGAR NOTIFICACIÓN SIGNALR ==========
            await _notificationService.NotifyTaskCreated(new
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                Description = tarea.Description,
                Status = tarea.Status,
                DueDate = tarea.DueDate,
                Mensaje = "Nueva tarea creada"
            });
            // ================================================

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

            // ========== AGREGAR NOTIFICACIÓN SIGNALR ==========
            await _notificationService.NotifyTaskUpdated(new
            {
                Id = TareaExistente.Id,
                Titulo = TareaExistente.Titulo,
                Description = TareaExistente.Description,
                Status = TareaExistente.Status,
                DueDate = TareaExistente.DueDate,
                Mensaje = "Tarea actualizada"
            });
            // ================================================

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

            // ========== AGREGAR NOTIFICACIÓN SIGNALR ==========
            await _notificationService.NotifyTaskDeleted(id);
            // ================================================

            return Ok();
        }
    }

    // ========== MOVER AUTHCONTROLLER FUERA ==========
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly GeneradorDeToken _generadorToken;

        public AuthController(GeneradorDeToken generadorToken)
        {
            _generadorToken = generadorToken;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            if (login.Username == "admin" && login.Password == "1234")
            {
                var token = _generadorToken.GenerateJwtToken(login.Username);
                return Ok(new { token });
            }
            return Unauthorized();
        }
    }
}