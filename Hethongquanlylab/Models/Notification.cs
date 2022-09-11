using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Hethongquanlylab.Models
{
    public class Notification
    {
        private int id;
        private string title;
        private string content;
        private string unit;
        private string date;
        private string link;

        public int ID { get => id; set => id = value; }
        public string Title { get => title; set => title = value; }
        public string Content { get => content; set => content = value; }
        public string Unit { get => unit; set => unit = value; }
        public string Date { get => date; set => date = value; }
        public string Link { get => link; set => link = value; }

        public Notification(int id, string title, string content,string unit, string date, string link)
        {
            this.ID = id;
            this.Title = title;
            this.Content = content;
            this.Unit = unit;
            this.Date = date;
            this.Link = link;
        }

        public Notification(DataRow row)
        {
            this.ID = (int)row["id"];
            this.Title = (string)row["title"];
            this.Content = (string)row["content"];
        }
    }
}
