using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColaDeAtencionAlCliente.Models
{
    public class QueueMannager
    {
        public List<PeopleQueue> Queues { get; set; }

        public void AddPerson(Person person)
        {
            QueueWithLessTime().AddPerson(person);
        }

        private PeopleQueue QueueWithLessTime()
        {
            return Queues.Min();
        }

        public void InitiateTimers()
        {
            Queues.ForEach(q => q.StartTimer());
        }
    }
}
