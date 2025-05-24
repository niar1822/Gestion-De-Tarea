

namespace GestionDeTareas.Models
{
    public interface Itarea
    {
        int Id { get; set; }
        string Titulo { get; set; }
        string Description { get; set; }
        DateTime DueDate { get; set; }
        string Status { get; set; }
        object ExtraData { get; set; }

        Task EjecutarAsync();
    }
}
