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
        public string LabID { get => labID; set => labID = value; }
        public string Name { get => name; set => name = value; }

        public User(string id, string name, string gen, string phoneNumber, string mail, string university, string subject)
        {
            this.LabID = id;
            this.Name = name;
        }
        public User(DataRow row)
        {
            this.LabID = (string)row["idMenu"];
            this.Name = (string)row["nameMenu"];
        }
    }

}
