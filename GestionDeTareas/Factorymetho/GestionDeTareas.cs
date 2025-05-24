using GestionDeTareas.Models;

namespace GestionDeTareas.Factorymetho
{
    public class GestionDeTareas
    {
        public static class TareaFactory
        {
            public static Itarea Crear( string tipo, string titulo, string descripcion, DateTime fecha, string estado, object extra)
            {
                return tipo.ToLower() switch
                {
                    "simple" => new TareaSimple
                    {
                        Titulo = titulo,
                        Description = descripcion,
                        DueDate = fecha,
                        Status = estado,
                        ExtraData = extra?.ToString()
                    },
                    "prioridad" => new TareaPrioridad
                    {
                        
                        Titulo = titulo,
                        Description = descripcion,
                        DueDate = fecha,
                        Status = estado,
                        ExtraData = (string)extra
                    },
                    _ => throw new ArgumentException("Tipo de tarea no válido")
                };
            }
        }
    }
}
