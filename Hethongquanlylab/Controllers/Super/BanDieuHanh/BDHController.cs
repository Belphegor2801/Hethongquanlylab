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
    public class BDHController : Controller
    {

        [HttpPost]
        public IActionResult FeedbackProcedure(String BDHfeedback, String IsSendToApproval)
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var ID = urlPath.ToString().Split('/').Last();
            Procedure newProcedure = ProcedureDAO.Instance.GetProcedureModel_Excel(ID);
            if (IsSendToApproval == "y")
            {
                ProcedureDAO.Instance.BDHApproval(newProcedure, BDHfeedback);
            }
            else
            {
                ProcedureDAO.Instance.BDHFeedbackProcedure(newProcedure, BDHfeedback);
            }
            return RedirectToAction("Procedure");

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

            var unit = "Ban Điều hành";
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

            return View("./Views/BDH/Content/Notification.cshtml", itemList);
        }
        [HttpPost]
        public IActionResult Notification(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Notification", "BDH", new { sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }
        public IActionResult AddNotification()
        {
            return View("./Views/BDH/Add/AddNotification.cshtml");
        }

        [HttpPost]
        public IActionResult AddNotification(String Title, String Content, String Date, String Link)
        {
            int ID = NotificationDAO.Instance.GetMaxID() + 1;
            var newNotification = new Notification(ID, Title, Content, "Ban Điều hành", Date, Link);
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

            var unit = "Ban Điều hành"; // unit
            var newNotification = new Notification(ID, Title, Content.ToString(), unit, Date, Link);
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

            return View("./Views/BDH/Content/Project.cshtml", itemList);
        }
        [HttpPost]
        public IActionResult Project(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Project", "BDH", new { sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }
        public IActionResult AddProject()
        {
            return View("./Views/BDH/AddProject.cshtml");
        }

        [HttpPost]
        public IActionResult AddProject(String Name, String StartDay, String Endday, String ProjectType, String Unit, String Status)
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
        public IActionResult Training()
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

            ItemDisplay<Training> trainingList = new ItemDisplay<Training>();
            trainingList.SortOrder = sortOrder;
            trainingList.CurrentSearchField = searchField;
            trainingList.CurrentSearchString = searchString;
            trainingList.CurrentPage = currentPage;

            List<Training> trainings = TrainingDAO.Instance.GetTrainingList_Excel();
            trainings = Function.Instance.searchItems(trainings, trainingList);
            trainings = Function.Instance.sortItems(trainings, trainingList.SortOrder);

            trainingList.Paging(trainings, 10);

            if (trainingList.PageCount > 0)
            {
                if (trainingList.CurrentPage > trainingList.PageCount) trainingList.CurrentPage = trainingList.PageCount;
                if (trainingList.CurrentPage < 1) trainingList.CurrentPage = 1;
                if (trainingList.CurrentPage != trainingList.PageCount)
                    try
                    {
                        trainingList.Items = trainingList.Items.GetRange((trainingList.CurrentPage - 1) * trainingList.PageSize, trainingList.PageSize);
                    }
                    catch { }

                else
                    trainingList.Items = trainingList.Items.GetRange((trainingList.CurrentPage - 1) * trainingList.PageSize, trainingList.Items.Count % trainingList.PageSize == 0 ? trainingList.PageSize : trainingList.Items.Count % trainingList.PageSize);
            }

            return View("./Views/BDH/Content/Training.cshtml", trainingList);
        }
        [HttpPost]
        public IActionResult Training(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Training", "BDH", new { sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }
        public IActionResult TrainingDetail()
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();
            var currenId = Convert.ToInt32(CurrentID);

            var training = TrainingDAO.Instance.GetTrainingModelbyId_Excel(currenId);
            return View("./Views/BDH/Detail/TrainingDetail.cshtml", training);
        }
        public IActionResult ExportTrainingToExcel()
        {
            List<Training> training = TrainingDAO.Instance.GetTrainingList_Excel();
            var stream = Function.Instance.ExportToExcel<Training>(training);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Danh sách bài đào tạo.xlsx");
        }
    }
}
