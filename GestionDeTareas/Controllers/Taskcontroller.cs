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

        public Taskcontroller(TaskContext context, TaskQueue taskQueue)
        {
            _Context = context;
            _taskQueue = taskQueue;
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

            _taskQueue.Enqueue(tareaGenerica);

            accionespost(tarea);

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

        [HttpPut]
        [Route("editar")]
        public async Task<IActionResult> EditarTarea(int id, TaskData tarea)
        {
            var TareaExistente = await _Context.Tasks.FindAsync(id);

            TareaExistente!.Description = tarea.Description;
            TareaExistente.DueDate = tarea.DueDate;
            TareaExistente.Status = tarea.Status;
            TareaExistente.ExtraData = tarea.ExtraData;

            await _Context.SaveChangesAsync();
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
            return Ok();
        }
    }
}
