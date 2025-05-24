using TaksModels.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GestionDeTareas.Models
{
    public class TareaSimple: Tarea<string>, Itarea
    {
        object Itarea.ExtraData
        {
            get => ExtraData!;
            set => ExtraData = value?.ToString() ?? string.Empty;
        }

        public Task EjecutarAsync()
        {
            throw new NotImplementedException();
        }
    }
}
