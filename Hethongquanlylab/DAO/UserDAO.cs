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
        public List<Member> GetInformationUserbyID(int id) // Lấy ra thông tin thành viên theo LabID
        {
            List<Member> list = new List<Member>();
            string tablename = "dbo.tblMenu";
            string query = "select * from " + tablename + "where idMenu = " + id;
            DataTable data = DataProvider.Instance.ExcuteQuery(query);
            foreach (DataRow dr in data.Rows)
            {
                Member user = new Member(dr);
                list.Add(user);
            }
            return list;
        }
        public List<Member> GetListUser() // thống kê ra 1 list các User
        {
            List<Member> list = new List<Member>();
            string tablename = "dbo.tblMenu";
            string query = "select * from " + tablename;
            DataTable data = DataProvider.Instance.ExcuteQuery(query);
            foreach (DataRow dr in data.Rows)
            {
                Member user = new Member(dr);
                list.Add(user);
            }
            return list;
        }
        public List<Member> GetListUserByPT(string ptname) // thống kê 1 List các User theo PowerTeam
        {
            List<Member> list = new List<Member>();
            string tablename = "dbo.tblMenu";
            string query = "select * from " + tablename;
            DataTable data = DataProvider.Instance.ExcuteQuery(query);
            foreach (DataRow dr in data.Rows)
            {
                Member user = new Member(dr);
                list.Add(user);
            }
            return list;
        }
        public List<Member> GetListUserbyGroup(string groupname) // thống kê list các User theo ban
        {
            List<Member> list = new List<Member>();
            string tablename = "dbo.tblMenu";
            string query = "select * from " + tablename + "where ";
            DataTable data = DataProvider.Instance.ExcuteQuery(query);
            foreach (DataRow dr in data.Rows)
            {
                Member user = new Member(dr);
                list.Add(user);
            }
            return list;
        }


        private ExcelPackage OpenFile()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/members.xlsx"));
            return package;
        }

        private void resetKey()
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                workSheet.Cells[i, 1].Value = i;
                i++;
            }
            package.Save();
        }

        private Member LoadData(ExcelWorksheet workSheet, int row)
        {
            var j = 1;
            var key = workSheet.Cells[row, j++].Value;
            var labid = workSheet.Cells[row, j++].Value;
            var avt = workSheet.Cells[row, j++].Value;
            var name = workSheet.Cells[row, j++].Value;
            var sex = workSheet.Cells[row, j++].Value;
            var birthday = workSheet.Cells[row, j++].Value;
            var gen = workSheet.Cells[row, j++].Value;
            var specialization = workSheet.Cells[row, j++].Value;
            var university = workSheet.Cells[row, j++].Value;
            var phone = workSheet.Cells[row, j++].Value;
            var mail = workSheet.Cells[row, j++].Value;
            var address = workSheet.Cells[row, j++].Value;
            var unit = workSheet.Cells[row, j++].Value;
            var position = workSheet.Cells[row, j++].Value;
            var isLT = workSheet.Cells[row, j++].Value;
            var isPassPTBT = workSheet.Cells[row, j++].Value;

            string Key = key.ToString();
            string LabID = labid == null ? "N/A" : labid.ToString();
            string AVT = avt == null ? "default.jpg" : avt.ToString();
            string Name = name == null ? "N/A" : name.ToString();
            string Sex = sex == null ? "N/A" : sex.ToString();
            string Birthday = birthday == null ? "N/A" : birthday.ToString();
            try
            {
                double date = Convert.ToDouble(Birthday);
                DateTimeFormatInfo fmt = (new CultureInfo("fr-FR")).DateTimeFormat;
                Birthday = DateTime.FromOADate(date).ToString("d", fmt);
            }
            catch { }
            string Gen = gen == null ? "N/A" : gen.ToString();
            string Specialization = specialization == null ? "N/A" : specialization.ToString();
            string University = university == null ? "N/A" : university.ToString();
            string Phone = phone == null ? "N/A" : phone.ToString();
            string Mail = mail == null ? "N/A" : mail.ToString();
            string Address = address == null ? "N/A" : address.ToString();
            string Unit = unit == null ? "N/A" : unit.ToString();
            string Position = position == null ? "N/A" : position.ToString();
            bool IsLT;
            try { IsLT = isLT == null ? false : Convert.ToBoolean(isLT.ToString()); }
            catch { IsLT = false; }
            bool IsPassPTBT;
            try { IsPassPTBT = isPassPTBT == null ? false : Convert.ToBoolean(isPassPTBT.ToString()); }
            catch { IsPassPTBT = false; }

            Member member = new Member(LabID, AVT, Name, Sex, Birthday, Gen, Specialization, University, Phone, Mail, Address, Unit, Position, IsLT, IsPassPTBT, Key);
            return member;
        }

        public List<Member> GetListUser_Excel()
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            List<Member> members = new List<Member>();

            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                Member member = LoadData(workSheet, i);
                members.Add(member);
            }
            return members;
        }

        public List<Member> GetListUser_Excel(string UnitVar)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            List<Member> members = new List<Member>();

            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                if (UnitVar == "LT")
                {
                    var isLT = workSheet.Cells[i, 15].Value;
                    bool IsLT;
                    try { IsLT = isLT == null ? false : Convert.ToBoolean(isLT.ToString()); }
                    catch { IsLT = false; }
                    if (IsLT)
                    {
                        Member member = LoadData(workSheet, i);
                        members.Add(member);
                    }
                }
                else if (UnitVar == "PT")
                {
                    var unit = workSheet.Cells[i, 13].Value;
                    string Unit = unit == null ? "N/A" : unit.ToString();
                    if (Unit.Contains("PT") || Unit.Contains("PowerTeam"))
                    {
                        Member member = LoadData(workSheet, i);
                        members.Add(member);
                    }
                }
                else
                {
                    var unit = workSheet.Cells[i, 13].Value;
                    string Unit = unit == null ? "N/A" : unit.ToString();
                    if (Unit.Contains(UnitVar))
                    {
                        Member member = LoadData(workSheet, i);
                        members.Add(member);
                    }
                }

            }
            return members;
        }

        public Member GetUserByID_Excel(string ID)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            List<Member> members = new List<Member>();
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                var key = workSheet.Cells[i, 1].Value;
                string Key = key == null ? "N/A" : key.ToString();
                if (Key == ID)
                {
                    Member member = LoadData(workSheet, i);
                    return member;
                }
            }
            return null;
        }

        public void AddMember(Member member)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                i++;
            }

            int j = 1;
            workSheet.Cells[i, j++].Value = i;
            workSheet.Cells[i, j++].Value = member.LabID;
            workSheet.Cells[i, j++].Value = member.Avt;
            workSheet.Cells[i, j++].Value = member.Name;
            workSheet.Cells[i, j++].Value = member.Sex;
            workSheet.Cells[i, j++].Value = member.Birthday;
            workSheet.Cells[i, j++].Value = member.Gen;
            workSheet.Cells[i, j++].Value = member.Specialization;
            workSheet.Cells[i, j++].Value = member.Univeristy;
            workSheet.Cells[i, j++].Value = member.Phone;
            workSheet.Cells[i, j++].Value = member.Email;
            workSheet.Cells[i, j++].Value = member.Address;
            workSheet.Cells[i, j++].Value = member.Unit;
            workSheet.Cells[i, j++].Value = member.Position;
            workSheet.Cells[i, j++].Value = member.IsLT;
            workSheet.Cells[i, j++].Value = member.IsPassPTBT;
            package.Save();
        }

        public void EditMember(Member member)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                var key = workSheet.Cells[i, 1].Value;
                string Key = key == null ? "N/A" : key.ToString();
                if (Key == member.Key)
                {
                    break;
                }
                i++;
            }

            int j = 2;

            workSheet.Cells[i, j++].Value = member.LabID;
            workSheet.Cells[i, j++].Value = member.Avt;
            workSheet.Cells[i, j++].Value = member.Name;
            workSheet.Cells[i, j++].Value = member.Sex;
            workSheet.Cells[i, j++].Value = member.Birthday;
            workSheet.Cells[i, j++].Value = member.Gen;
            workSheet.Cells[i, j++].Value = member.Specialization;
            workSheet.Cells[i, j++].Value = member.Univeristy;
            workSheet.Cells[i, j++].Value = member.Phone;
            workSheet.Cells[i, j++].Value = member.Email;
            workSheet.Cells[i, j++].Value = member.Address;
            workSheet.Cells[i, j++].Value = member.Unit;
            workSheet.Cells[i, j++].Value = member.Position;
            workSheet.Cells[i, j++].Value = member.IsLT;
            workSheet.Cells[i, j++].Value = member.IsPassPTBT;
            package.Save();
        }

        public void DeleteMember(String id)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                var key = workSheet.Cells[i, 1].Value;
                string Key = key == null ? "N/A" : key.ToString();
                if (Key == id)
                {
                    break;
                }
                i++;
            }
            workSheet.DeleteRow(i);
            resetKey();
            package.Save();
        }

        public void DeleteMemberFromUnit(String ID, String Unit)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                var labID = workSheet.Cells[i, 1].Value;
                string LabID = labID == null ? "N/A" : labID.ToString();
                if (LabID == ID)
                {
                    break;
                }
                i++;
            }
            try
            {
                var units = workSheet.Cells[i, 13].Value.ToString();
                var unit = units.Split(",");
                var newUnits = new List<string>();
                foreach (var item in unit)
                {
                    if (!unit.Contains(Unit))
                    {
                        newUnits.Add(item);
                    }
                }
                if (newUnits.Count > 1)
                {
                    workSheet.Cells[i, 13].Value = string.Join(",", newUnits);
                }
                else if (newUnits.Count == 1)
                {
                    workSheet.Cells[i, 13].Value = newUnits[0];
                }
                else
                {
                    workSheet.Cells[i, 13].Value = "Chưa có";
                }
            }
            catch
            {
                workSheet.Cells[i, 13].Value = "Chưa có";
            }
            package.Save();
        }
    }
}
