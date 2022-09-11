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
    public class BDTController : Controller
    {
        public IActionResult Index()
        {
            String page;
            var urlQuery = Request.HttpContext.Request.Query;
            page = urlQuery["page"];
            page = page == null ? "1" : page;
            int currentPage = Convert.ToInt32(page);
            ItemDisplay<Notification> itemList = new ItemDisplay<Notification>();
            itemList.CurrentPage = currentPage;

            List<Notification> items = NotificationDAO.Instance.GetNotificationList_Excel();

            itemList.Paging(items, 5);

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
            return View("./Views/BDT/BDTHome.cshtml", itemList);
        }

        public IActionResult NotificationDetail()
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();
            var currenId = Convert.ToInt32(CurrentID);

            var notification = NotificationDAO.Instance.GetNotificationModelbyId_Excel(currenId);
            return View("./Views/BDT/NotificationDetail.cshtml", notification);
        }

        private List<Member> sortMember(List<Member> members, String sortOrder)
        {
            switch (sortOrder)
            {
                case "id_desc":
                    members = members.OrderByDescending(s => Convert.ToInt32(s.LabID)).ToList();
                    break;
                case "Name":
                    members = members.OrderBy(s => s.Name.Split(" ").Last()).ToList();
                    break;
                case "name_desc":
                    members = members.OrderByDescending(s => s.Name.Split(" ").Last()).ToList();
                    break;
                case "Gen":
                    members = members.OrderBy(s => s.Gen).ToList();
                    break;
                case "gen_desc":
                    members = members.OrderByDescending(s => s.Gen).ToList();
                    break;
                case "Birthday":
                    members = members.OrderBy(s => s.Birthday.Split("/").Last()).ToList();
                    break;
                case "birthday_desc":
                    members = members.OrderByDescending(s => s.Birthday.Split("/").Last()).ToList();
                    break;

                default:
                    members = members.OrderBy(s => Convert.ToInt32(s.LabID)).ToList();
                    break;
            }
            return members;
        }

        private List<Member> searchMember(List<Member> members, ItemDisplay<Member> memberList)
        {
            if (!String.IsNullOrEmpty(memberList.CurrentSearchField))
            {
                if (!String.IsNullOrEmpty(memberList.CurrentSearchString))
                {
                    switch (memberList.CurrentSearchField)
                    {
                        case "Lab ID":
                            members = members.Where(s => s.LabID.Contains(memberList.CurrentSearchString)).ToList();
                            break;
                        case "Name":
                            members = members.Where(s => s.Name.Contains(memberList.CurrentSearchString)).ToList();
                            break;
                        case "Sex":
                            members = members.Where(s => s.Sex.Contains(memberList.CurrentSearchString)).ToList();
                            break;
                        case "Birthday":
                            members = members.Where(s => s.Birthday.Contains(memberList.CurrentSearchString)).ToList();
                            break;
                        case "Gen":
                            members = members.Where(s => s.Gen.Contains(memberList.CurrentSearchString)).ToList();
                            break;
                        case "Unit":
                            members = members.Where(s => s.Unit.Contains(memberList.CurrentSearchString)).ToList();
                            break;
                        case "Position":
                            members = members.Where(s => s.Position.Contains(memberList.CurrentSearchString)).ToList();
                            break;
                        default:
                            members = members.Where(s => s.LabID.Contains(memberList.CurrentSearchString)).ToList();
                            break;
                    }
                }
            }
            return members;
        }


        public IActionResult ExportToExcel()
        {
            var memoryStream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(memoryStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("Danh sách thành viên");
                var currentRow = 1;
                // trỏ đến dòng 1 và cột 1 thay giá trị bằng LabID các dòng dưới cx tương tự


                var allAttr = typeof(Member).GetProperties(); // Lấy danh sách attributes của class Member
                int col = 1;
                foreach (var attr in allAttr)
                    worksheet.Cells[currentRow, col++].Value = attr.Name;

                // Lấy tất cả dữ liệu trong database theo thứ tự tăng dần labID
                List<Member> members = UserDAO.Instance.GetListUser_Excel();
                foreach (var member in members)
                {
                    // Dòng thứ 2 trở đi sẽ đổ dữ liệu từ database vào
                    currentRow += 1;
                    col = 1;
                    foreach (var attr in allAttr)
                    {
                        object value = attr.GetValue(member);
                        worksheet.Cells[currentRow, col++].Value = value.ToString();
                    }
                }
                // Trả về dữ liệu dạng xlsx
                using (var stream = new MemoryStream())
                {
                    excelPackage.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachThanhVienBanDaoTao.xlsx");
                }
            }
        }



        public IActionResult Member()
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

            sortOrder = sortOrder == null ? "LabID" : sortOrder; ;
            searchField = searchField == null ? "LabID" : searchField;
            searchString = searchString == null ? "" : searchString;
            page = page == null ? "1" : page;
            int currentPage = Convert.ToInt32(page);

            ItemDisplay<Member> memberList = new ItemDisplay<Member>();
            memberList.SortOrder = sortOrder;
            memberList.CurrentSearchField = searchField;
            memberList.CurrentSearchString = searchString;
            memberList.CurrentPage = currentPage;

            List<Member> members = UserDAO.Instance.FindMemberbyUnit("PT");
            members = Function.Instance.searchItems(members, memberList);
            members = Function.Instance.sortItems(members, memberList.SortOrder);

            memberList.Paging(members, 10);

            if (memberList.PageCount > 0)
            {
                if (memberList.CurrentPage > memberList.PageCount) memberList.CurrentPage = memberList.PageCount;
                if (memberList.CurrentPage < 1) memberList.CurrentPage = 1;
                if (memberList.CurrentPage != memberList.PageCount)
                    try
                    {
                        memberList.Items = memberList.Items.GetRange((memberList.CurrentPage - 1) * memberList.PageSize, memberList.PageSize);
                    }
                    catch { }

                else
                    memberList.Items = memberList.Items.GetRange((memberList.CurrentPage - 1) * memberList.PageSize, memberList.Items.Count % memberList.PageSize == 0 ? memberList.PageSize : memberList.Items.Count % memberList.PageSize);
            }

            return View("./Views/BDT/Member.cshtml", memberList);
        }

        [HttpPost]
        public IActionResult Member(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Member", "BDT", new { sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }


        public IActionResult MemberDetail()
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();

            var user = UserDAO.Instance.GetUserByID_Excel(CurrentID);
            return View("./Views/Shared/MemberDetail.cshtml", user);
        }
        public IActionResult AddMember()
        {
            return View("./Views/BDT/AddMember.cshtml");
        }

        public IActionResult Procedure()
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

            ItemDisplay<Procedure> procedureList = new ItemDisplay<Procedure>();
            procedureList.SortOrder = sortOrder;
            procedureList.CurrentSearchField = searchField;
            procedureList.CurrentSearchString = searchString;
            procedureList.CurrentPage = currentPage;

            List<Procedure> procedures = ProcedureDAO.Instance.GetProcedureList_Excel();
            procedures = Function.Instance.searchItems(procedures, procedureList);
            procedures = Function.Instance.sortItems(procedures, procedureList.SortOrder);

            procedureList.Paging(procedures, 10);

            if (procedureList.PageCount > 0)
            {
                if (procedureList.CurrentPage > procedureList.PageCount) procedureList.CurrentPage = procedureList.PageCount;
                if (procedureList.CurrentPage < 1) procedureList.CurrentPage = 1;
                if (procedureList.CurrentPage != procedureList.PageCount)
                    try
                    {
                        procedureList.Items = procedureList.Items.GetRange((procedureList.CurrentPage - 1) * procedureList.PageSize, procedureList.PageSize);
                    }
                    catch { }

                else
                    procedureList.Items = procedureList.Items.GetRange((procedureList.CurrentPage - 1) * procedureList.PageSize, procedureList.Items.Count % procedureList.PageSize == 0 ? procedureList.PageSize : procedureList.Items.Count % procedureList.PageSize);
            }

            return View("./Views/BDT/Procedure.cshtml", procedureList);
        }
        [HttpPost]
        public IActionResult Procedure(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Procedure", "BDT", new { sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }
        public IActionResult AddProcedure()
        {
            return View("./Views/BDT/AddProcedure.cshtml");
        }

        [HttpPost]
        public IActionResult AddProcedure(String Name, String Content, String Link)
        {
            int ID = ProcedureDAO.Instance.GetMaxID() + 1;
            var userSession = JsonConvert.DeserializeObject<UserLogin>(HttpContext.Session.GetString("LoginSession"));
            var unit = userSession.UserName; // unit
            var newProcedure = new Procedure(ID, Name, unit, Content.ToString(), Link);
            ProcedureDAO.Instance.AddProcedure(newProcedure);
            return RedirectToAction("Procedure");
        }
        public IActionResult ExportProcedureToExcel()
        {
            var memoryStream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(memoryStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("Danh sách quy trình Ban Đào tạo");
                var currentRow = 1;
                // trỏ đến dòng 1 và cột 1 thay giá trị bằng LabID các dòng dưới cx tương tự


                var allAttr = typeof(Procedure).GetProperties(); // Lấy danh sách attributes của class Member
                int col = 1;
                foreach (var attr in allAttr)
                    worksheet.Cells[currentRow, col++].Value = attr.Name;

                // Lấy tất cả dữ liệu trong database theo thứ tự tăng dần labID
                List<Procedure> procedures = ProcedureDAO.Instance.GetProcedureList_Excel();
                foreach (var procedure in procedures)
                {
                    // Dòng thứ 2 trở đi sẽ đổ dữ liệu từ database vào
                    currentRow += 1;
                    col = 1;
                    foreach (var attr in allAttr)
                    {
                        object value = attr.GetValue(procedure);
                        worksheet.Cells[currentRow, col++].Value = value.ToString();
                    }
                }
                // Trả về dữ liệu dạng xlsx
                using (var stream = new MemoryStream())
                {
                    excelPackage.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachQuytrinhBanDaoTao.xlsx");
                }
            }
        }
        public IActionResult DeleteProcedure()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String ProcedureId_delete = urlQuery["procedureID"];
            ProcedureDAO.Instance.DeleteProcedure(ProcedureId_delete);

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

            return View("./Views/BDT/Training.cshtml", trainingList);
        }
        [HttpPost]
        public IActionResult Training(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Training", "BDT", new { sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }
        public IActionResult TrainingDetail()
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();
            var currenId = Convert.ToInt32(CurrentID);

            var training = TrainingDAO.Instance.GetTrainingModelbyId_Excel(currenId);
            return View("./Views/BDT/TrainingDetail.cshtml", training);
        }
        public IActionResult ExportTrainingToExcel()
        {
            List<Training> training = TrainingDAO.Instance.GetTrainingList_Excel();
            var stream = Function.Instance.ExportToExcel<Training>(training);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Danh sách bài đào tạo.xlsx");
        }
    }
}
