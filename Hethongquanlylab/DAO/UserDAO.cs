﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Hethongquanlylab.Models;
using OfficeOpenXml;
using System.IO;

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
        public List<User> GetInformationUserbyID(int id) // Lấy ra thông tin thành viên theo LabID
        {
            List<User> list = new List<User>();
            string tablename = "dbo.tblMenu";
            string query = "select * from " + tablename + "where idMenu = " + id;
            DataTable data = DataProvider.Instance.ExcuteQuery(query);
            foreach (DataRow dr in data.Rows)
            {
                User user = new User(dr);
                list.Add(user);
            }
            return list;
        }
        public List<User> GetListUser() // thống kê ra 1 list các User
        {
            List<User> list = new List<User>();
            string tablename = "dbo.tblMenu";
            string query = "select * from " + tablename;
            DataTable data = DataProvider.Instance.ExcuteQuery(query);
            foreach (DataRow dr in data.Rows)
            {
                User user = new User(dr);
                list.Add(user);
            }
            return list;
        }
        public List<User> GetListUserByPT(string ptname) // thống kê 1 List các User theo PowerTeam
        {
            List<User> list = new List<User>();
            string tablename = "dbo.tblMenu";
            string query = "select * from " + tablename;
            DataTable data = DataProvider.Instance.ExcuteQuery(query);
            foreach (DataRow dr in data.Rows)
            {
                User user = new User(dr);
                list.Add(user);
            }
            return list;
        }

        public List<User> GetListUserbyGroup(string groupname) // thống kê list các User theo ban
        {
            List<User> list = new List<User>();
            string tablename = "dbo.tblMenu";
            string query = "select * from " + tablename + "where ";
            DataTable data = DataProvider.Instance.ExcuteQuery(query);
            foreach (DataRow dr in data.Rows)
            {
                User user = new User(dr);
                list.Add(user);
            }
            return list;
        }
        public List<User> GetListUser_Excel()
        {
            List<User> userList = new List<User>();// mở file excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("user.csv"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                int j = 1;

                string labID = workSheet.Cells[i, j++].Value.ToString();
                string name = workSheet.Cells[i, j++].Value.ToString();
                string sex = workSheet.Cells[i, j++].Value.ToString();
                string birthday = workSheet.Cells[i, j++].Value.ToString();
                string gen = workSheet.Cells[i, j++].Value.ToString();
                string unit = workSheet.Cells[i, j++].Value.ToString();
                string position = workSheet.Cells[i, j++].Value.ToString();
                User user = new User(labID, name, sex, birthday, gen, unit, position);
                userList.Add(user);
            }
            return userList;
        }
        public int EditUserInfomtion_Excel(string id, string name, string sex, string birthday, string gen, string unit, string position)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("user.csv"));
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
                    return 1;
                }    
            }
            return 0;
        }
    }
}
