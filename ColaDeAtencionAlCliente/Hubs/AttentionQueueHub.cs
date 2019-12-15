using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ColaDeAtencionAlCliente.Models;
using System;

namespace ColaDeAtencionAlCliente.Hubs
{
    public class AttentionQueueHub : Hub
    {
        public async override Task OnConnectedAsync()
        {
            await Task.Run(() => {
                Program.mannager.Queues.ForEach(async (queue) =>
                {
                    await Clients.Caller.SendAsync(queue.Name, Utf8Json.JsonSerializer.ToJsonString(queue.People.ToArray()));
                });
            });
        }

        public async Task AddPerson(string id, string name)
        {
            Person person = new Person() { Cedula = int.Parse(id), Name = name, Attended = false, Date = DateTime.Now };
            Program.mannager.AddPerson(person);
            await Clients.All.SendAsync("AddedToQueue", id, name);
        }
    }
}
