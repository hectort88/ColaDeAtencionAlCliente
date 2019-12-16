using System;
using System.Collections.Generic;
using System.Linq;
using ColaDeAtencionAlCliente.Hubs;
using ColaDeAtencionAlCliente.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace ColaDeAtencionAlCliente
{
    public class Program
    {
        public static IHubContext<AttentionQueueHub> hubContext;
        public static string connectionString;
        public static QueueMannager mannager;
        
        public static void Main(string[] args)
        {

            InitializeQueues();
            
            CreateHostBuilder(args).Build().Run();
        }

        public static async void EnviarData(string chanel, string data)
        {
            System.Diagnostics.Debug.Write(data);
            if (hubContext != null)
            {
                await hubContext.Clients.All.SendAsync(chanel, data);
            }
        }

        public static void InitializeQueues()
        {
            // Dos colas con tiempos de atencion disferente.
            PeopleQueue queue1 = new PeopleQueue() { Interval = TimeSpan.FromMinutes(2), Name = "queue1", People = new Queue<Person>() };
            PeopleQueue queue2 = new PeopleQueue() { Interval = TimeSpan.FromMinutes(3), Name = "queue2", People = new Queue<Person>() };

            mannager = new QueueMannager
            {
                Queues = new List<PeopleQueue>
            {
                queue1,
                queue2
            }
            };

            mannager.InitiateTimers();

            // Cargar persona desde la base de datos que no han sido atendidas.
            using var db = new PersonContext();
            var people = db.People.ToList().Where(p => !p.Attended);
            foreach (var p in people)
            {
                mannager.AddPerson(p);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
