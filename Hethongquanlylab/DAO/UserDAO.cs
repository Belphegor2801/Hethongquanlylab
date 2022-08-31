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


        public List<Member> GetListUser_Excel()
        {
            List<Member> userList = new List<Member>();// mở file excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/users.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            int i = 3;
            while(workSheet.Cells[i, 1].Value != null)
            {
                string labID = workSheet.Cells[i, 1].Value.ToString();
                string name = workSheet.Cells[i, 2].Value.ToString();
                string sex = workSheet.Cells[i, 3].Value.ToString();
                string sDate = (workSheet.Cells[i, 4].Value).ToString();
                string birthday;
                try
                {
                    double date = Convert.ToDouble(sDate);
                    DateTimeFormatInfo fmt = (new CultureInfo("fr-FR")).DateTimeFormat;
                    birthday = DateTime.FromOADate(date).ToString("d", fmt);
                }
                catch
                {
                    birthday = sDate;
                }
                string gen = workSheet.Cells[i, 5].Value.ToString();
                string unit = workSheet.Cells[i, 6].Value.ToString();
                string position = workSheet.Cells[i, 7].Value.ToString();
                Member user = new Member(labID, name, sex, birthday, gen, unit, position);
                userList.Add(user);
                i++;
            }
            return userList;
        }
        public Member GetUserByID_Excel(string ID)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/users.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            int i = 3;
            while (workSheet.Cells[i, 1].Value != null)
            {
                int j = 1;
                string labID = workSheet.Cells[i, 1].Value.ToString();
                if (labID == ID)
                {
                    string name = workSheet.Cells[i, 2].Value.ToString();
                    string sex = workSheet.Cells[i, 3].Value.ToString();
                    string sDate = (workSheet.Cells[i, 4].Value).ToString();
                    string birthday;
                    try
                    {
                        double date = Convert.ToDouble(sDate);
                        DateTimeFormatInfo fmt = (new CultureInfo("fr-FR")).DateTimeFormat;
                        birthday = DateTime.FromOADate(date).ToString("d", fmt);
                    }
                    catch
                    {
                        birthday = sDate;
                    }
                    string gen = workSheet.Cells[i, 5].Value.ToString();
                    string unit = workSheet.Cells[i, 6].Value.ToString();
                    string position = workSheet.Cells[i, 7].Value.ToString();
                    Member user = new Member(labID, name, sex, birthday, gen, unit, position);
                    return user;
                }
                i++;
            }
            return null;
        }

        public void EditUserInfomtion_Excel(string id, string name, string sex, string birthday, string gen, string unit, string position)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/users.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                int j = 1;
                string labID = workSheet.Cells[i, j++].Value.ToString();
                if(labID == id)
                {
                    workSheet.Cells[i, j++].Value = name;
                    workSheet.Cells[i, j++].Value = sex;
                    workSheet.Cells[i, j++].Value = birthday;
                    workSheet.Cells[i, j++].Value = gen;
                    workSheet.Cells[i, j++].Value = unit;
                    workSheet.Cells[i, j++].Value = position;
                    break;
                }    
            }
            package.Save();
        }
    }
}
