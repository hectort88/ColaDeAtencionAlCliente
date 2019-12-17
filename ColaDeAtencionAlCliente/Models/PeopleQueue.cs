using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ColaDeAtencionAlCliente.Models
{
    public class PeopleQueue : IComparable<PeopleQueue>
    {
        public Queue<Person> People { get; set; }
        public TimeSpan Interval { get; set; }
        public string Name { get; set; }
        private int SecondsLeft { get; set; }

        public int CompareTo([AllowNull] PeopleQueue other)
        {
            return TimeToWait().CompareTo(other.TimeToWait());
        }

        public void AddPerson(Person person)
        {
            if (person.ID == 0)
            {
                using var db = new PersonContext();
                db.Add(person);
                db.SaveChanges();
                System.Diagnostics.Debug.Write(person.ID);
            }
            People.Enqueue(person);
            SendQueueInJsonFormat();
        }

        public void RemovePerson()
        {
            if (People.Count() > 0)
            {
                Person person = People.Dequeue();
                person.Attended = true;
                using var db = new PersonContext();
                db.Update(person);
                db.SaveChanges();
                SendQueueInJsonFormat();
            }
        }

        public double TimeToWait()
        {
            if (People.Count() == 0) return 0;
            return ((People.Count() - 1) * Interval.TotalSeconds) + SecondsLeft;
        }

        public void SendQueueInJsonFormat()
        {
            var peopleArray = People.ToArray();
            Program.EnviarData(this.Name, Utf8Json.JsonSerializer.ToJsonString(peopleArray));
        }

        public void StartTimer()
        {
            SecondsLeft = (int)Interval.TotalSeconds;
            Task.Run(async () =>
            {
                while (true)
                {
                    if (People.Count > 0)
                    {
                        await Task.Delay(1000);
                        SecondsLeft -= 1;
                        if (SecondsLeft <= 0)
                        {
                            RemovePerson();
                            SecondsLeft = (int)Interval.TotalSeconds;
                        }
                    }
                }
            });
        }
    }
}
