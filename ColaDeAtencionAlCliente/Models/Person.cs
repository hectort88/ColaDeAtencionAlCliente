using System;

namespace ColaDeAtencionAlCliente.Models
{
    public class Person
    {
        public int ID { get; set; }
        public int Cedula { get; set; }
        public string Name { get; set; }
        public bool Attended { get; set; }
        public DateTime Date { get; set; }

    }
}
