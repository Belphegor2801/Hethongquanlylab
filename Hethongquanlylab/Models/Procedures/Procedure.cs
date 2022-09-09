using System;
using System.Globalization;

namespace Hethongquanlylab.Models
{
    public class Procedure
    {
        private int id;
        private string name;
        private string unit;
        private string senddate;
        private string content;
        private bool v1;
        private bool v2;
        private bool v3;
        private string status;
        private string link;
        

        public int ID { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Unit { get => unit; set => unit = value; }
        public string Senddate { get => senddate; set => senddate = value; }
        public string Content { get => content; set => content = value; }
        public bool V1 { get => v1; set => v3 = value; }
        public bool V2 { get => v2; set => v3 = value; }
        public bool V3 { get => v3; set => v3 = value; }
        public string Status { get => status; set => status = value; }
        public string Link { get => link; set => link = value; }

        public Procedure(int id, string name, string unit, string content, string link) // Thêm mới
        {
            this.ID = id;
            this.Name = name;
            this.Unit = unit;
            this.Link = link;
            this.Content = content;

            DateTime day = DateTime.Today;
            DateTimeFormatInfo fmt = (new CultureInfo("fr-FR")).DateTimeFormat;
            string senddate = day.ToString("d", fmt);

            this.Senddate = senddate;

            this.V1 = false;
            this.V2 = false;
            this.V3 = false;

            this.Status = "Chưa duyệt";
        }

        public Procedure(int id, string name, string unit, string senddate, string content, string v1, string v2, string v3, string link) // Load từ excel
        {
            this.ID = id;
            this.Name = name;
            this.Unit = unit;
            this.Senddate = senddate;
            this.Link = link;
            this.Content = content;

            this.V1 = Convert.ToBoolean(v1);
            this.V2 = Convert.ToBoolean(v2);
            this.V3 = Convert.ToBoolean(v3);

            if ((!this.V1) && (!this.V2) && (!this.V3))
                this.Status = "Chưa duyệt";
            else if ((this.V1) && (this.V2) && (this.V3))
                this.Status = "Đã duyệt";
            else
                this.Status = "Đang duyệt";

        }

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
