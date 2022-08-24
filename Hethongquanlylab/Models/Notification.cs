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
        private string image;

        public int Id { get => id; set => id = value; }
        public string Title { get => title; set => title = value; }
        public string Content { get => content; set => content = value; }
        public string Image { get => image; set => image = value; }

        public Notification(int id, string title, string content, string image)
        {
            this.Id = id;
            this.Title = title;
            this.Content = content;
            this.Image = image;
        }

        public Notification(DataRow row)
        {
            this.Id = (int)row["id"];
            this.Title = (string)row["title"];
            this.Content = (string)row["content"];
            this.Image = (string)row["image"];
        }
    }
}
