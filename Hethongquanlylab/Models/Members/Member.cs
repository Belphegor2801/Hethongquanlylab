using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Hethongquanlylab.Models
{
    public class Member
    {
        private string labID;
        private string avt;
        private string name;
        private string sex;
        private string birthday;
        private string gen;
        private string unit;
        private string position;

        public string LabID { get => labID; set => labID = value; }
        public string Avt { get => avt; set => avt = value; }
        public string Name { get => name; set => name = value; }
        public string Sex { get => sex; set => sex = value; }
        public string Birthday { get => birthday; set => birthday = value; }
        public string Gen { get => gen; set => gen = value; }
        public string Unit { get => unit; set => unit = value; }
        public string Position { get => position; set => position = value; }

        public Member(string id, string avt, string name, string sex, string birthday, string gen, string unit, string position)
        {
            this.LabID = id;
            this.Avt = avt;
            this.Name = name;
            this.Sex = sex;
            this.Birthday = birthday;
            this.Gen = gen;
            this.Unit = unit;
            this.Position = position;
        }
        public Member(DataRow row)
        {
            this.LabID = (string)row["idMenu"];
            this.Name = (string)row["nameMenu"];
        }
    }

}
