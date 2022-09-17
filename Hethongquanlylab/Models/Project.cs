using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hethongquanlylab.Models
{
    public class Project
    {
        private string id;
        private string name;
        private string startday;
        private string endday;
        private string projectType;
        private string status;
        private string unit;
        private string subID;

        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Startday { get => startday; set => startday = value; }
        public string Endday { get => endday; set => endday = value; }
        public string ProjectType { get => projectType; set => projectType = value; }
        public string Status { get => status; set => status = value; }
        public string Unit { get => unit; set => unit = value; }
        public string SubID { get => subID; set => subID = value; }

        public Project(string id, string subid, string name, string startday, string endday, string projectType, string status, string unit)
        {
            this.Id = id;
            this.Name = name;
            this.Startday = startday;
            this.Endday = endday;
            this.ProjectType = projectType;
            this.Status = status;
            this.Unit = unit;
            this.SubID = subid;
        }
    }
}
