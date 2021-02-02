using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace finalASP.Models
{
    public class Asset
    {
        private int id;
        private string name;
        private string description;
        private int value; 
        private int userId;

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public int Value { get => value; set => this.value = value; }
        public int UserId { get => userId; set => userId = value; }

        public Asset(int id, string name, string description, int value, int userId)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.value = value;
            this.userId = userId;
        }

        public Asset()
        {
        }
    }
}