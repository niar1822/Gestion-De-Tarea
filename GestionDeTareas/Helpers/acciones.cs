using TaksModels.Models;

namespace GestionDeTareas.Helpers
{
    public static class Acciones
    {
        public static void NotificarCreacion(TaskData tarea)
        {
            Console.WriteLine($"[notificación] Se ha creado la tarea: {tarea.Titulo}");
        }
    }
}
