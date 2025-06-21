using Microsoft.EntityFrameworkCore;
using TaksModels.Models;

namespace GestionDeTareas.Models
{
    public class TaskContext:DbContext
    {
        public TaskContext(DbContextOptions<TaskContext> options) : base(options)
        {
        
        }
        
        public DbSet<TaskData> Tasks { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}
