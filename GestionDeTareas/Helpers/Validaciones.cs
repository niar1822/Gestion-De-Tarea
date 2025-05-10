using TaksModels.Models;

namespace GestionDeTareas.Helpers
{
    public delegate bool ValidarCampo(TaskData tarea);
    public class Validaciones
    {
        public static bool ValidarDescripcion(TaskData tarea)
            => !string.IsNullOrEmpty(tarea.Description) && tarea.Description.Length <= 100 && !string.IsNullOrEmpty(tarea.Titulo);
        public static bool ValidarFecha(TaskData tarea)
            => tarea.DueDate > DateTime.Now;

    }
}
