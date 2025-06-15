using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;

namespace GestionDeTareas.huds
{
    public class TaskHub : Hub
    {
        // Método para que los clientes se unan a un grupo (opcional)
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        // Método para que los clientes salgan de un grupo (opcional)
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        // Se ejecuta cuando un cliente se conecta
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Cliente conectado: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        // Se ejecuta cuando un cliente se desconecta
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Cliente desconectado: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
