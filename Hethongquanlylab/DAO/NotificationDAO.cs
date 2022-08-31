using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hethongquanlylab.Models;
using OfficeOpenXml;

namespace Hethongquanlylab.DAO
{
    public class NotificationDAO
    {
        private static NotificationDAO instance;
        public static NotificationDAO Instance
        {
            get { if (instance == null) instance = new NotificationDAO(); return NotificationDAO.instance; }
            private set { NotificationDAO.instance = value; }
        }

        private NotificationDAO() { }

        public List<Notification> GetNotificationList_Excel()
        {
            List<Notification> notificationList = new List<Notification>();// mở file excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/files/data.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                int j = 1;
                int id = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                string title = workSheet.Cells[i, j++].Value.ToString();
                string content = workSheet.Cells[i, j++].Value.ToString();
                string image = workSheet.Cells[i, j++].Value.ToString();
                Notification notification = new Notification(id, title, content, image);
                notificationList.Add(notification);
            }
            return notificationList;
        }

        public Notification GetNotificationModelbyId_Excel(int notificationid)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/files/training.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                int j = 1;
                int id = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                if (id == notificationid)
                {
                    string title = workSheet.Cells[i, j++].Value.ToString();
                    string content = workSheet.Cells[i, j++].Value.ToString();
                    string image = workSheet.Cells[i, j++].Value.ToString();
                    Notification notification = new Notification(id, title, content, image);
                    return notification;
                }
            }
            return null;
        }

        public List<Member> FindMemberbyName(string nameUser)
        {
            List<Member> memberList = new List<Member>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/files/user.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            int i = 3;
            while (workSheet.Cells[i, 1].Value != null)
            {
                string name = workSheet.Cells[i, 2].Value.ToString();
                if (name.Contains(nameUser))
                {
                    string labID = workSheet.Cells[i, 1].Value.ToString();
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
                    memberList.Add(user);
                }
                i++;
            }
            return memberList;
        }
        public List<Member> FindMemberbyGen(string Gen)
        {
            List<Member> memberList = new List<Member>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/files/user.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            int i = 3;
            while (workSheet.Cells[i, 1].Value != null)
            {
                string gen = workSheet.Cells[i, 5].Value.ToString();
                if (Gen == gen)
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
                    string unit = workSheet.Cells[i, 6].Value.ToString();
                    string position = workSheet.Cells[i, 7].Value.ToString();
                    Member user = new Member(labID, name, sex, birthday, gen, unit, position);
                    memberList.Add(user);
                }
                i++;
            }
            return memberList;
        }
        public List<Member> FindMemberbyUnit(string Unit)
        {
            List<Member> memberList = new List<Member>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/files/user.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            int i = 3;
            while (workSheet.Cells[i, 1].Value != null)
            {
                string unit = workSheet.Cells[i, 6].Value.ToString();
                if (unit == Unit )
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
                    string position = workSheet.Cells[i, 7].Value.ToString();
                    Member user = new Member(labID, name, sex, birthday, gen, unit, position);
                    memberList.Add(user);
                }
                i++;
            }
            return memberList;
    }
}
