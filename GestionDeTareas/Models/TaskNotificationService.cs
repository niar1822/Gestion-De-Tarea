using GestionDeTareas.huds;
using GestionDeTareas.Models;
using Microsoft.AspNetCore.SignalR;

namespace TuProyecto.Services
{
    public class TaskNotificationService : ITaskNotificationService
    {
        private readonly IHubContext<TaskHub> _hubContext;

        public TaskNotificationService(IHubContext<TaskHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyTaskCreated(object task)
        {
            await _hubContext.Clients.All.SendAsync("TaskCreated", task);
        }

        public async Task NotifyTaskUpdated(object task)
        {
            await _hubContext.Clients.All.SendAsync("TaskUpdated", task);
        }

        public async Task NotifyTaskDeleted(int taskId)
        {
            await _hubContext.Clients.All.SendAsync("TaskDeleted", taskId);
        }
    }
}

