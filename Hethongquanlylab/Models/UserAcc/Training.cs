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
        private string date;
        private string unit;
        private string content;

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Link { get => link; set => link = value; }
        public string Date { get => date; set => date = value; }
        public string Unit { get => unit; set => unit = value; }
        public string Content { get => content; set => content = value; }

        public Training (int id, string name, string link, string date, string unit, string content)
        {
            this.Id = id;
            this.Name = name;
            this.Link = link;
            this.Date = date;
            this.Unit = unit;
            this.Content = content;
        }
    }
}
