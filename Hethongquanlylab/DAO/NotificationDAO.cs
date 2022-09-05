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
                int j = 1;
                int id = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                string title = workSheet.Cells[i, j++].Value.ToString();
                string content = workSheet.Cells[i, j++].Value.ToString();
                string image = workSheet.Cells[i, j++].Value.ToString();
                string unit = workSheet.Cells[i, j++].Value.ToString();
                string sDate = (workSheet.Cells[i, j++].Value).ToString();
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
                string link = workSheet.Cells[i, j++].Value.ToString();
                Notification notification = new Notification(id, title, content, image, unit, date, link);
                notificationList.Add(notification);
            }
            return notificationList;
        }

        public Notification GetNotificationModelbyId_Excel(int notificationid)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/notification"));
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
                    string unit = workSheet.Cells[i, j++].Value.ToString();
                    string sDate = (workSheet.Cells[i, j++].Value).ToString();
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
                    string link = workSheet.Cells[i, j++].Value.ToString();
                    Notification notification = new Notification(id, title, content, image, unit, date, link);
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
                    string image = workSheet.Cells[i, 4].Value.ToString();
                    string unit = workSheet.Cells[i, 5].Value.ToString();
                    string sDate = (workSheet.Cells[i, 6].Value).ToString();
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
                    string link = workSheet.Cells[i, 7].Value.ToString();
                    Notification notification = new Notification(id, title, content, image, unit, date, link);
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
            int i = 3;
            while (workSheet.Cells[i, 1].Value != null)
            {
                string unit = workSheet.Cells[i, 5].Value.ToString();
                if (Unit == unit)
                {
                    int id = Convert.ToInt32(workSheet.Cells[i, 1].Value);
                    string title = workSheet.Cells[i, 2].Value.ToString();
                    string content = workSheet.Cells[i, 3].Value.ToString();
                    string image = workSheet.Cells[i, 4].Value.ToString();
                    string sDate = (workSheet.Cells[i, 6].Value).ToString();
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
                    string link = workSheet.Cells[i, 7].Value.ToString();
                    Notification notification = new Notification(id, title, content, image, unit, date, link);
                    notificationList.Add(notification);
                }
                i++;
            }
            return notificationList;
        }

    }
}
