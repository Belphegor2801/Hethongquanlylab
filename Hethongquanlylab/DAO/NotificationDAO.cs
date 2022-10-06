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
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/notification.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                int id = Convert.ToInt32(workSheet.Cells[i, 1].Value);
                string title = workSheet.Cells[i, 2].Value.ToString();
                string content = workSheet.Cells[i, 3].Value.ToString();
                string unit = workSheet.Cells[i, 4].Value.ToString();
                string sDate = (workSheet.Cells[i, 5].Value).ToString();
                string date;
                try
                {
                    double Date = Convert.ToDouble(sDate);
                    DateTimeFormatInfo fmt = (new CultureInfo("fr-FR")).DateTimeFormat;
                    date = DateTime.FromOADate(Date).ToString("d", fmt);
                }
                catch
                {
                    date = sDate;
                }
                string link = workSheet.Cells[i, 6].Value == null? "none": workSheet.Cells[i, 6].Value.ToString(); ;
                Notification notification = new Notification(id, title, content, unit, date, link);
                notificationList.Add(notification);
            }
            return notificationList;
        }

        public Notification GetNotificationModelbyId_Excel(int notificationid)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/notification.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                int j = 1;
                int id = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                if (id == notificationid)
                {
                    string title = workSheet.Cells[i, 2].Value.ToString();
                    string content = workSheet.Cells[i, 3].Value.ToString();
                    string unit = workSheet.Cells[i, 4].Value.ToString();
                    string sDate = (workSheet.Cells[i, 5].Value).ToString();
                    string date;
                    try
                    {
                        double Date = Convert.ToDouble(sDate);
                        DateTimeFormatInfo fmt = (new CultureInfo("fr-FR")).DateTimeFormat;
                        date = DateTime.FromOADate(Date).ToString("d", fmt);
                    }
                    catch
                    {
                        date = sDate;
                    }
                    string link = workSheet.Cells[i, 6].Value == null ? "none" : workSheet.Cells[i, 6].Value.ToString(); ;
                    Notification notification = new Notification(id, title, content, unit, date, link);
                    return notification;
                }
            }
            return null;
        }

        public List<Notification> FindNotificationbyTitle(string notificationTitle)
        {
            List<Notification> notificationList = new List<Notification>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/notification.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            int i = 3;
            while (workSheet.Cells[i, 1].Value != null)
            {
                string title = workSheet.Cells[i, 2].Value.ToString();
                if (title.Contains(notificationTitle))
                {
                    int id = Convert.ToInt32(workSheet.Cells[i, 1].Value);
                    string content = workSheet.Cells[i, 3].Value.ToString();
                    string unit = workSheet.Cells[i, 4].Value.ToString();
                    string sDate = (workSheet.Cells[i, 5].Value).ToString();
                    string date;
                    try
                    {
                        double Date = Convert.ToDouble(sDate);
                        DateTimeFormatInfo fmt = (new CultureInfo("fr-FR")).DateTimeFormat;
                        date = DateTime.FromOADate(Date).ToString("d", fmt);
                    }
                    catch
                    {
                        date = sDate;
                    }
                    string link = workSheet.Cells[i, 6].Value.ToString();
                    Notification notification = new Notification(id, title, content, unit, date, link);
                    notificationList.Add(notification);
                }
                i++;
            }
            return notificationList;
        }
        public List<Notification> GetNotificationListbyUnit(string Unit)
        {
            List<Notification> notificationList = new List<Notification>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/notification.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                string unit = workSheet.Cells[i, 4].Value.ToString();
                if (Unit == unit)
                {
                    int id = Convert.ToInt32(workSheet.Cells[i, 1].Value);
                    string title = workSheet.Cells[i, 2].Value.ToString();
                    string content = workSheet.Cells[i, 3].Value.ToString();
                    string sDate = (workSheet.Cells[i, 5].Value).ToString();
                    string date;
                    try
                    {
                        double Date = Convert.ToDouble(sDate);
                        DateTimeFormatInfo fmt = (new CultureInfo("fr-FR")).DateTimeFormat;
                        date = DateTime.FromOADate(Date).ToString("d", fmt);
                    }
                    catch
                    {
                        date = sDate;
                    }
                    var link = workSheet.Cells[i, 6].Value;
                    string Link = link == null? "": link.ToString();
                    Notification notification = new Notification(id, title, content, unit, date, Link);
                    notificationList.Add(notification);
                }
                i++;
            }
            return notificationList;
        }
        public void DeleteNotification(String id)
        {
            List<Notification> notificationList = new List<Notification>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/notification.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                string Id = workSheet.Cells[i, 1].Value.ToString();
                if (id == Id)
                {
                    break;
                }
                i++;
            }
            workSheet.DeleteRow(i);
            package.Save();
        }
        public void AddNotification(Notification notification)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/notification.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i = 3;
            while (workSheet.Cells[i, 1].Value != null)
            {
                i++;
            }

            int lastRow = i;
            workSheet.Cells[lastRow, 1].Value = notification.ID;
            workSheet.Cells[lastRow, 2].Value = notification.Title;
            workSheet.Cells[lastRow, 3].Value = notification.Content;
            workSheet.Cells[lastRow, 4].Value = notification.Unit;
            workSheet.Cells[lastRow, 5].Value = DateTime.Now.ToString("dd/MM/yyyy hh:mm");
            workSheet.Cells[lastRow, 6].Value = notification.Link;
            package.Save();
        }
        public int GetMaxID()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/notification.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            return workSheet.Dimension.End.Row;
        }
        public void EditNotification(Notification notification)
        {
            List<Notification> notificationList = new List<Notification>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/notification.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i;
            for (i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                string Id = workSheet.Cells[i, 1].Value.ToString();
                if (notification.ID.ToString() == Id)
                {
                    break;
                }
            }
            workSheet.Cells[i, 1].Value = notification.ID;
            workSheet.Cells[i, 2].Value = notification.Title;
            workSheet.Cells[i, 3].Value = notification.Content;
            workSheet.Cells[i, 5].Value = DateTime.Now.ToString("dd/mm/yyyy HH:mm:ss");
            workSheet.Cells[i, 6].Value = notification.Link;
            package.Save();
        }
    }
}
