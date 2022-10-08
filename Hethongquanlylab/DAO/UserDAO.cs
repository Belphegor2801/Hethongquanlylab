using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Hethongquanlylab.Models;
using OfficeOpenXml;
using System.IO;
using System.Globalization;

namespace Hethongquanlylab.DAO
{
    public class UserDAO
    {
        private static UserDAO instance;
        public static UserDAO Instance
        {
            get { if (instance == null) instance = new UserDAO(); return UserDAO.instance; }
            private set { UserDAO.instance = value; }
        }

        private UserDAO() { }

        public List<Member> GetListUser()
        {
            List<Member> members = DataProvider<Member>.Instance.GetListItem();
            return members;
        }

        public List<Member> GetListUser(string UnitVar)
        {
            List<Member> members = new List<Member>();
            if (UnitVar == "PT")
            {
                List<Member> memberList = DataProvider<Member>.Instance.GetListItem();
                members.AddRange(memberList.Where(s => CultureInfo.CurrentCulture.CompareInfo.IndexOf(s.Unit, "PT", CompareOptions.IgnoreCase) >= 0).ToList());
                members.AddRange(memberList.Where(s => CultureInfo.CurrentCulture.CompareInfo.IndexOf(s.Unit, "PowerTeam", CompareOptions.IgnoreCase) >= 0).ToList());
                members.AddRange(memberList.Where(s => CultureInfo.CurrentCulture.CompareInfo.IndexOf(s.Unit, "Power Team", CompareOptions.IgnoreCase) >= 0).ToList());
            }
            else if (UnitVar == "LT") members = DataProvider<Member>.Instance.GetListItem("IsLT", "1");
            else if (UnitVar == "All") members = DataProvider<Member>.Instance.GetListItem();
            else members = DataProvider<Member>.Instance.GetListItem("Unit", UnitVar);
            return members;
        }

        public Member GetUserByID(string ID)
        {
            Member member = DataProvider<Member>.Instance.GetItem("Key", ID);
            return member;
        }

        public Member GetUserByLabID(string LabID)
        {
            Member member = DataProvider<Member>.Instance.GetItem("LabID", LabID);
            return member;
        }

        public void AddMember(Member member)
        {
            DataTable data = DataProvider<Member>.Instance.LoadData();
            DataRow newMember = data.NewRow();

            var allAttr = typeof(Member).GetProperties(); // Lấy danh sách attributes của class Member

            foreach (var attr in allAttr)
                newMember[attr.Name] = attr.GetValue(member);


            data.Rows.Add(newMember);

            DataProvider<Member>.Instance.UpdateData(data);
        }

        public void EditMember(Member member)
        {
            DataTable data = DataProvider<Member>.Instance.LoadData();
            DataRow newMember = data.Select("Key=" + member.Key).FirstOrDefault();

            if (newMember != null)
            {
                var allAttr = typeof(Member).GetProperties(); // Lấy danh sách attributes của class Member
                foreach (var attr in allAttr)
                    newMember[attr.Name] = attr.GetValue(member);
            }
           
            DataProvider<Member>.Instance.UpdateData(data);
        }

        public void DeleteMember(String Key)
        {
            DataProvider<Member>.Instance.DeleteItem("Key", Key);
        }

        public void DeleteMemberFromUnit(String ID, String Unit)
        {
            DataTable data = DataProvider<Member>.Instance.LoadData();
            DataRow newMember = data.Select("Key=" + ID).FirstOrDefault();
            try
            {
                var unit = newMember["Unit"].ToString();
                var units = unit.Split(",");
                var newUnits = new List<string>();
                foreach (var item in units)
                {
                    if (!item.Contains(Unit))
                    {
                        newUnits.Add(item);
                    }
                }

                if (newUnits.Count > 1)
                {
                    newMember["Unit"] = string.Join(",", newUnits);
                }
                else if (newUnits.Count == 1)
                {
                    newMember["Unit"] = newUnits[0];
                }
                else
                {
                    newMember["Unit"] = "Không";
                }
            }
            catch
            {
                newMember["Unit"] = "Không";
            }
            DataProvider<Member>.Instance.UpdateData(data);
        }
    }
}
