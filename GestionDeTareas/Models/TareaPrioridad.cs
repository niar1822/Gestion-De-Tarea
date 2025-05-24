using TaksModels.Models;

namespace GestionDeTareas.Models
{
    public class TareaPrioridad : Tarea<string>, Itarea
    {
        object Itarea.ExtraData
        {
            get => ExtraData!;
            set => ExtraData = value.ToString();
        }

        public Task EjecutarAsync()
        {
            throw new NotImplementedException();
        }
    }
}
