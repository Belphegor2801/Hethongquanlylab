using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hethongquanlylab.Models
{
    public class Procedure
    {
        private int id;
        private string name;
        private string link;
        private string unit;
        private string status;

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Link { get => link; set => link = value; }
        public string Unit { get => unit; set => unit = value; }
        public string Status { get => status; set => status = value; }

        public Procedure(int id, string name, string link, string unit, string status)
        {
            this.Id = id;
            this.Name = name;
            this.Link = link;
            this.Unit = unit;
            this.Status = status;
        }
    }
}
