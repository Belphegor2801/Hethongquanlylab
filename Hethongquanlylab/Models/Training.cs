using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Hethongquanlylab.Models
{
    public class Training
    {
        private int id;
        private string name;
        private string link;

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Link { get => link; set => link = value; }

        public Training (int id, string name, string link)
        {
            this.Id = id;
            this.Name = name;
            this.Link = link;
        }
        public Training(DataRow row)
        {
            this.Id = (int)row["id"];
            this.Name = (string)row["name"];
            this.Link = (string)row["link"];
        }
    }
}
