using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace Hethongquanlylab.Models
{
    public class Procedure
    {
        private string id;
        private string subid;
        private string name;
        private string unit;
        private string senddate;
        private string content;
        private bool v1;
        private string bdhReply;
        private bool v2;
        private string bcvReply;
        private bool v3;
        private string nslReply;
        private string status;
        private string link;
        private Dictionary<DateTime, String> eventLog;

        public string ID { get => id; set => id = value; }
        public string SubID { get => subid; set => subid = value; }
        public string Name { get => name; set => name = value; }
        public string Unit { get => unit; set => unit = value; }
        public string Senddate { get => senddate; set => senddate = value; }
        public string Content { get => content; set => content = value; }
        public bool V1 { get => v1; set => v1 = value; }
        public bool V2 { get => v2; set => v2 = value; }
        public bool V3 { get => v3; set => v3 = value; }
        public string Status { get => status; set => status = value; }
        public string Link { get => link; set => link = value; }
        public string BdhReply { get => bdhReply; set => bdhReply = value; }
        public string BcvReply { get => bcvReply; set => bcvReply = value; }
        public string NSLReply { get => nslReply; set => nslReply = value; }
        public Dictionary<DateTime, String> EventLog { get => eventLog; set => eventLog = value; }

        public static Dictionary<string, string> ColorVar { get; set; }

        public Procedure(string name, string unit, string content, string link, string id = "1", string subid = "SubID") // Thêm mới + chỉnh sửa
        {
            this.ID = id;
            this.SubID = subid;
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
        public Procedure(string id, string name, string unit, string content, string bdh, string bcv, string link) // Phản hồi
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

        public Procedure(string id, string subid, string name, string unit, string senddate, string content, bool v1, string bdh,  bool v2, string bcv, bool v3, string nsl, string status, string link) // Load từ excel
        {
            this.ID = id;
            this.SubID = subid;
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
            this.NSLReply = nsl;
            this.Status = status;

            ColorVar = new Dictionary<string, string>();
            ColorVar.Add("Chưa duyệt", "#4800ff");
            ColorVar.Add("Chờ duyệt", "#ff6a00");
            ColorVar.Add("Ban Điều Hành đã duyệt", "#0a0");
            ColorVar.Add("Ban Cố Vấn đã duyệt", "#0c0");
            ColorVar.Add("Nhà Sáng Lập đã duyệt", "#0f0");
            ColorVar.Add("Ban Điều Hành trả lại", "#a00");
            ColorVar.Add("Ban Cố Vấn trả lại", "#c00");
            ColorVar.Add("Nhà Sáng Lập trả lại", "#f00");
        }
    }
}
