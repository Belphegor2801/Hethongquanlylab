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
        private string labId;
        private string startday;
        private string endday;
        private string projectType;
        private string status;

        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string LabId { get => labId; set => labId = value; }
        public string Startday { get => startday; set => startday = value; }
        public string Endday { get => endday; set => endday = value; }
        public string ProjectType { get => projectType; set => projectType = value; }
        public string Status { get => status; set => status = value; }
        

        public Project(string id, string name, string labid, string startday, string endday, string projectType, string status)
        {
            this.Id = id;
            this.Name = name;
            this.LabId = labid;
            this.Startday = startday;
            this.Endday = endday;
            this.ProjectType = projectType;
            this.Status = status;
        }
    }
}
