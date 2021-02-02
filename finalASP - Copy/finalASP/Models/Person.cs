using Microsoft.Ajax.Utilities;

namespace finalASP.Models
{
    public class Person
    {
        private int id;  
        private string name;
        private string role;

        public Person() { }
        public Person(int id1, string name1, string password1 ) {
            Id = id1;
            Name = name1;
            Role = password1;
        }

        public string Name { get => name; set => name = value; }
        public string Role { get => role; set => role = value; }
        public int Id { get => id; set => id = value; }
    }
}