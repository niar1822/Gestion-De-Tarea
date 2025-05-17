using TaksModels.Models;

namespace GestionDeTareas.Helpers
{
    public class Calculardias
    {
        public static Func<TaskData, int> CalcularDiasRestantes = (tarea) =>
        {
            var diasRestantes = (tarea.DueDate - DateTime.Now).Days;

            return diasRestantes;
        };
    }
}
