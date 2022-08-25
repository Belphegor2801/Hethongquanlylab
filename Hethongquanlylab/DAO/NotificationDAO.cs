using System;
using System.Collections.Generic;
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
    }
}
