using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hethongquanlylab.DAO;
using Hethongquanlylab.Models;
using System.IO;
using Hethongquanlylab.Common;
using OfficeOpenXml;
using Newtonsoft.Json;
using Hethongquanlylab.Models.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Hethongquanlylab.Controllers.Super.BanDaoTao
{
    public class BDTController : SuperController
    {
        public BDTController()
        {
            unit = "Ban Đào Tạo";
            unitVar = "BDT";
        }

        public IActionResult Notification()
        {
            String sortOrder;
            String searchField;
            String searchString;
            String page;

            var urlQuery = Request.HttpContext.Request.Query;
            sortOrder = urlQuery["sort"];
            searchField = urlQuery["searchField"];
            searchString = urlQuery["SearchString"];
            page = urlQuery["page"];

            sortOrder = sortOrder == null ? "ID" : sortOrder; ;
            searchField = searchField == null ? "ID" : searchField;
            searchString = searchString == null ? "" : searchString;
            page = page == null ? "1" : page;
            int currentPage = Convert.ToInt32(page);

            var unit = "Ban Đào tạo";
            ItemDisplay<Notification> itemList = new ItemDisplay<Notification>();
            itemList.SortOrder = sortOrder;
            itemList.CurrentSearchField = searchField;
            itemList.CurrentSearchString = searchString;
            itemList.CurrentPage = currentPage;

            List<Notification> items = NotificationDAO.Instance.GetNotificationListbyUnit(unit);
            items = Function.Instance.searchItems(items, itemList);
            items = Function.Instance.sortItems(items, itemList.SortOrder);

            itemList.Paging(items, 10);

            if (itemList.PageCount > 0)
            {
                if (itemList.CurrentPage > itemList.PageCount) itemList.CurrentPage = itemList.PageCount;
                if (itemList.CurrentPage < 1) itemList.CurrentPage = 1;
                if (itemList.CurrentPage != itemList.PageCount)
                    try
                    {
                        itemList.Items = itemList.Items.GetRange((itemList.CurrentPage - 1) * itemList.PageSize, itemList.PageSize);
                    }
                    catch { }

                else
                    itemList.Items = itemList.Items.GetRange((itemList.CurrentPage - 1) * itemList.PageSize, itemList.Items.Count % itemList.PageSize == 0 ? itemList.PageSize : itemList.Items.Count % itemList.PageSize);
            }

            return View("./Views/BDT/Notification.cshtml", itemList);
        }
        [HttpPost]
        public IActionResult Notification(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Notification", "BDT", new { sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }
        public IActionResult AddNotification()
        {
            return View("./Views/BDT/AddNotification.cshtml");
        }

        [HttpPost]
        public IActionResult AddNotification(String Title, String Content,  String Date, String Link)
        {
            int ID = NotificationDAO.Instance.GetMaxID() + 1;
            var newNotification = new Notification(ID, Title, Content, "Ban Đào tạo", Date, Link);
            NotificationDAO.Instance.AddNotification(newNotification);
            return RedirectToAction("Notification");
        }
        [HttpPost]
        public IActionResult EditNotification(String Title, String Content, String Date, String Link)
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();
            var ID = Convert.ToInt32(CurrentID);

            var unit = "Ban Đào tạo"; // unit
            var newNotification = new Notification(ID,Title,Content.ToString(),unit, Date, Link);
            NotificationDAO.Instance.EditNotification(newNotification);
            return RedirectToAction("Notification");
        }

        public IActionResult DeleteNotification()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String ID_delete = urlQuery["notiID"];
            NotificationDAO.Instance.DeleteNotification(ID_delete);
            return RedirectToAction("Notification");
        }
        public IActionResult Project()
        {
            String sortOrder;
            String searchField;
            String searchString;
            String page;

            var urlQuery = Request.HttpContext.Request.Query;
            sortOrder = urlQuery["sort"];
            searchField = urlQuery["searchField"];
            searchString = urlQuery["SearchString"];
            page = urlQuery["page"];

            sortOrder = sortOrder == null ? "ID" : sortOrder; ;
            searchField = searchField == null ? "ID" : searchField;
            searchString = searchString == null ? "" : searchString;
            page = page == null ? "1" : page;
            int currentPage = Convert.ToInt32(page);

            var unit = "BanDaoTao";
            ItemDisplay<Project> itemList = new ItemDisplay<Project>();
            itemList.SortOrder = sortOrder;
            itemList.CurrentSearchField = searchField;
            itemList.CurrentSearchString = searchString;
            itemList.CurrentPage = currentPage;

            List<Project> items = ProjectDAO.Instance.GetProjectList_Excel();
            items = Function.Instance.searchItems(items, itemList);
            items = Function.Instance.sortItems(items, itemList.SortOrder);

            itemList.Paging(items, 10);

            if (itemList.PageCount > 0)
            {
                if (itemList.CurrentPage > itemList.PageCount) itemList.CurrentPage = itemList.PageCount;
                if (itemList.CurrentPage < 1) itemList.CurrentPage = 1;
                if (itemList.CurrentPage != itemList.PageCount)
                    try
                    {
                        itemList.Items = itemList.Items.GetRange((itemList.CurrentPage - 1) * itemList.PageSize, itemList.PageSize);
                    }
                    catch { }

                else
                    itemList.Items = itemList.Items.GetRange((itemList.CurrentPage - 1) * itemList.PageSize, itemList.Items.Count % itemList.PageSize == 0 ? itemList.PageSize : itemList.Items.Count % itemList.PageSize);
            }

            return View("./Views/BDT/Project.cshtml", itemList);
        }
        [HttpPost]
        public IActionResult Project(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Project", "BDT", new { sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }
        public IActionResult AddProject()
        {
            return View("./Views/BDT/AddProject.cshtml");
        }


        [HttpPost]
        public IActionResult AddProject(String Name, String StartDay, String Endday,String ProjectType, String Unit, String Status)
        {
            string ID = ProjectDAO.Instance.GetMaxID() + 1;
            var newNotification = new Project(ID, Name, StartDay, Endday, ProjectType, Unit, Status);
            ProjectDAO.Instance.AddProject(newNotification);
            return RedirectToAction("Project");
        }
        public IActionResult DeleteProject()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String ID_delete = urlQuery["ID"];
            NotificationDAO.Instance.DeleteNotification(ID_delete);
            return RedirectToAction("Project");
        }
        public IActionResult ExportProjectToExcel()
        {
            List<Project> project = ProjectDAO.Instance.GetProjectList_Excel();
            var stream = Function.Instance.ExportToExcel<Project>(project);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachDuAn.xlsx");
        }
        
        
    }
}
