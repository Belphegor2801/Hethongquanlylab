using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

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
        private string bdhReply;
        private string bcvReply;
        

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
        public string BdhReply { get => bdhReply; set => bdhReply = value; }
        public string BcvReply { get => bcvReply; set => bcvReply = value; }

        public static Dictionary<string, string> ColorVar { get; set; }

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
            this.BdhReply = "Chưa có phản hồi";
            this.BcvReply = "Chưa có phản hồi";
        }
        public Procedure(int id, string name, string unit, string content, string bdh, string bcv, string link) // Phản hồi
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
            this.BdhReply = bdh;
            this.BcvReply = bcv;
        }

        public Procedure(int id, string name, string unit, string senddate, string content, string v1, string v2, string v3, string status, string link, string bdh, string bcv) // Load từ excel
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

            this.Status = status;
            this.BdhReply = bdh;
            this.BcvReply = bcv;

            ColorVar = new Dictionary<string, string>();
            ColorVar.Add("Chưa duyệt", "#4800ff");
            ColorVar.Add("Chờ duyệt", "#ff6a00");
            ColorVar.Add("Đã duyệt bởi Ban Điều Hành", "#0a0");
            ColorVar.Add("Trả lại", "#00f");

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
