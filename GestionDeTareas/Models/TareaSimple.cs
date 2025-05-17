using TaksModels.Models;

namespace GestionDeTareas.Models
{
    public class TareaSimple: Tarea<string>, Itarea
    {
        object Itarea.ExtraData
        {
            get => ExtraData;
            set => ExtraData = value?.ToString() ?? string.Empty;
        }
    }
}
