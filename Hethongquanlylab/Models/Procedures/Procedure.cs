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
        private string senddate;
        private string content;
        private string link;
        private string status;

        public int ID { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Link { get => link; set => link = value; }
        public string Status { get => status; set => status = value; }
        public string Senddate { get => senddate; set => senddate = value; }
        public string Content { get => content; set => content = value; }

        public Procedure(int id, string name, string senddate, string content, string link, string status)
        {
            this.ID = id;
            this.Name = name;
            this.Link = link;
            this.Senddate = senddate;
            this.Content = content; 
            this.Status = status;
        }
    }
}
