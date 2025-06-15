namespace GestionDeTareas.Models
{
    public interface ITaskNotificationService
    {
        Task NotifyTaskCreated(object task);
        Task NotifyTaskUpdated(object task);
        Task NotifyTaskDeleted(int taskId);
    }
}
