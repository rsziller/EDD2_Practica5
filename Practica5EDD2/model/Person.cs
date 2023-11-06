using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica5EDD2.model
{   //Clase persona con interfaz de comparador
    public class Person:IComparable<Person>
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("dpi")]
        public long Id { get; set; }
        [JsonProperty("dateBirth")]
        //public DateTime BirthDate { get; set; }
        public string BirthDate { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("companies")]
        public string[] Company { get; set; }

        [JsonProperty("recluiter")]
        public string Reclutador { get; set; }

        //public Person(string name, long id, DateTime birthDate, string address)
        public Person(string name, long id, string birthDate, string address)
        {
            Name = name;
            Id = id;
            BirthDate = birthDate;
            Address = address;
        }

        public Person()
        {

        }

        public int CompareTo(Person other)
        {

            return Id.CompareTo(other.Id);
        }
    }
}
