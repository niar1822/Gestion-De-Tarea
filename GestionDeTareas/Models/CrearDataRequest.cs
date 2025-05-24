namespace GestionDeTareas.Models
{
    public class CrearDataRequest
    {
        public string Tipo { get; set; } = "simple";
        public string? Titulo { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public string? Status { get; set; }
        public object? ExtraData { get; set; }
    }
}
