using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Hethongquanlylab.Models
{
    public class User
    {
        private int id;
        private string name;
        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }

        public User(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
        public User(DataRow row)
        {
            this.Id = (int)row["idMenu"];
            this.Name = (string)row["nameMenu"];
        }
    }

}
