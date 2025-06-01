using TaksModels.Models;

namespace GestionDeTareas.Models
{
    public static class MemoService<T>
    {
        private static readonly Dictionary<string, double> cachePorcentaje = new();
        private static readonly Dictionary<string, List<Tarea<T>>> cacheFiltroEstado = new();

        public static double ObtenerPorcentajeCompletadas(IEnumerable<Tarea<T>> tareas)
        {
            var tareasList = tareas.ToList(); // Solo convertimos aquí si hace falta más de una pasada
            string clave = string.Join(",", tareasList.Select(t => $"{t.Id}:{t.Status}:{t.Completada()}"));

            if (cachePorcentaje.ContainsKey(clave))
                return cachePorcentaje[clave];

            double completadas = tareasList.Count(t => t.Completada());
            double porcentaje = tareasList.Count == 0 ? 0 : (completadas / tareasList.Count) * 100;

            cachePorcentaje[clave] = porcentaje;
            return porcentaje;
        }

        public static List<Tarea<T>> FiltrarPorEstado(IEnumerable<Tarea<T>> tareas, string estado)
        {
            var tareasList = tareas.ToList();
            string clave = estado + ":" + string.Join(",", tareasList.Select(t => $"{t.Id}:{t.Status}"));

            if (cacheFiltroEstado.ContainsKey(clave))
                return cacheFiltroEstado[clave];

            var resultado = tareasList.Where(t => t.Status == estado).ToList();
            cacheFiltroEstado[clave] = resultado;
            return resultado;
        }

        public static void LimpiarCache()
        {
            cachePorcentaje.Clear();
            cacheFiltroEstado.Clear();
        }
    }
}
