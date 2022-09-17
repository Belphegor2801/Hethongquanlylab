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
        private string key;
        private string labID;
        private string avt;
        private string name;
        private string sex;
        private string birthday;
        private string gen;
        private string specialization;
        private string university;
        private string phone;
        private string email;
        private string address;
        private string unit;
        private string position;
        private bool isLT;
        private bool isPassPTBT;

        public string Key { get => key; set => key = value; }
        public string LabID { get => labID; set => labID = value; }
        public string Avt { get => avt; set => avt = value; }
        public string Name { get => name; set => name = value; }
        public string Sex { get => sex; set => sex = value; }
        public string Birthday { get => birthday; set => birthday = value; }
        public string Gen { get => gen; set => gen = value; }
        public string Specialization { get => specialization; set => specialization = value; }
        public string Univeristy { get => university; set => university = value; }
        public string Phone { get => phone; set => phone = value; }
        public string Email { get => email; set => email = value; }
        public string Address { get => address; set => address = value; }
        public string Unit { get => unit; set => unit = value; }
        public string Position { get => position; set => position = value; }
        public bool IsLT { get => isLT; set => isLT = value; }
        public bool IsPassPTBT { get => isPassPTBT; set => isPassPTBT = value; }

        public Member(string labid, string avt, string name, string sex, string birthday, string gen, string phone, string email, string address, string specilization, string university, string unit, string position, bool isLT, bool isPassPTBT, string key = "1")
        {
            this.Key = key;
            this.LabID = labid;
            this.Avt = avt;
            this.Name = name;
            this.Sex = sex;
            this.Birthday = birthday;
            this.Gen = gen;
            this.Phone = phone;
            this.Email = email;
            this.Address = address;
            this.Specialization = specilization;
            this.Univeristy = university;
            this.Unit = unit;
            this.Position = position;
            this.IsLT = isLT;
            this.isPassPTBT = isPassPTBT;
        }
        public Member( string avt, string name, string sex, string birthday, string phone, string email, string address, string specilization, string university, string key = "1")
        {
            this.Key = key;
            this.Avt = avt;
            this.Name = name;
            this.Sex = sex;
            this.Birthday = birthday;
            this.Gen = gen;
            this.Phone = phone;
            this.Email = email;
            this.Address = address;
            this.Specialization = specilization;
            this.Univeristy = university;
            this.Unit = unit;
            this.Position = position;
            this.IsLT = isLT;
            this.isPassPTBT = isPassPTBT;
        }
        public Member(DataRow row)
        {
            this.LabID = (string)row["idMenu"];
            this.Name = (string)row["nameMenu"];
        }
    }

}
