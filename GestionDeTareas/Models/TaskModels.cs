using System.ComponentModel.DataAnnotations;

namespace TaksModels.Models
{
    public class Tarea<T>
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = "simple";
        public string Titulo { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public T? ExtraData { get; set; }
    }

    public class TaskData : Tarea<string> 
    {
    }

}
