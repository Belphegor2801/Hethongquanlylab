using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Hethongquanlylab.Models;

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
    }
}
