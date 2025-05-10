using System.ComponentModel;
using System.Numerics;
using GestionDeTareas.Helpers;
using GestionDeTareas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaksModels.Models;

namespace GestionDeTareas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Taskcontroller : ControllerBase
    {
        private readonly TaskContext _Context;

        public Taskcontroller(TaskContext context)
        {
            _Context = context;
        }

        [HttpPost]
        [Route("crear")]
        public async Task<IActionResult> Creartarea(TaskData tarea)
        {
            ValidarCampo validarCampo = Validaciones.ValidarDescripcion;
            validarCampo += Validaciones.ValidarFecha;
            int diasrestantes = Calculardias.CalcularDiasRestantes(tarea);

            foreach (ValidarCampo item in validarCampo.GetInvocationList())
            {
                if (!item(tarea))
                {
                    return BadRequest("Error en la validacion de los campos");
                }
            }
            await _Context.Tasks.AddAsync(tarea);
            await _Context.SaveChangesAsync();

            Action<TaskData> accionespost = Acciones.NotificarCreacion;
            accionespost(tarea);

            return Ok(new
            {
                mensaje = "Tarea creada exitosamente",
                diasrestantes = diasrestantes,

            }
            );
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
