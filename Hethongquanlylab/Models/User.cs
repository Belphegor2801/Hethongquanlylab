using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Hethongquanlylab.Models
{
    public class User
    {
        private string labID;
        private string name;
        private string gen;
        private string sdt;
        private string email;
        private string truong;
        private string chuyennganh;

        public string LabID { get => labID; set => labID = value; }
        public string Name { get => name; set => name = value; }
        public string Gen { get => gen; set => gen = value; }
        public string Sdt { get => sdt; set => sdt = value; }
        public string Email { get => email; set => email = value; }
        public string Truong { get => truong; set => truong = value; }
        public string Chuyennganh { get => chuyennganh; set => chuyennganh = value; }

        public User(string id, string name, string gen, string sdt, string email, string truong, string chuyennganh)
        {
            this.LabID = id;
            this.Name = name;
            this.Gen = gen;
            this.Sdt = sdt;
            this.Email = email;
            this.Truong = truong;
            this.Chuyennganh = chuyennganh;
        }
        public User(DataRow row)
        {
            this.LabID = (string)row["idMenu"];
            this.Name = (string)row["nameMenu"];
            this.Gen = (string)row["Gen"];
            this.Truong = (string)row["Truong"];
            this.Chuyennganh = (string)row["Chuyennganh"];
        }
    }

}
