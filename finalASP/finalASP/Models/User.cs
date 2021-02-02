using Microsoft.Ajax.Utilities;

namespace finalASP.Models
{
    public class User
    {
        private int id;  
        private string name;
        private string password;

        public User() { }
        public User(int id1, string name1, string password1 ) {
            Id = id1;
            Name = name1;
            Password = password1;
        }

        public string Name { get => name; set => name = value; }
        public string Password { get => password; set => password = value; }
        public int Id { get => id; set => id = value; }
    }
}