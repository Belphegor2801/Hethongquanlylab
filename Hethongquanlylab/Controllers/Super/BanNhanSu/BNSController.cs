using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Hethongquanlylab.Models;
using Hethongquanlylab.DAO;
using Hethongquanlylab.Common;
using OfficeOpenXml;
using System.IO;
using System.Data;
using OfficeOpenXml.Table;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Hethongquanlylab.Models.Login;


namespace Hethongquanlylab.Controllers.Super.BanNhanSu
{
    public class BNSController : Controller
    {
        public IActionResult Index()
        {
            String page;
            var urlQuery = Request.HttpContext.Request.Query;
            page = urlQuery["page"];
            page = page == null ? "1" : page;
            int currentPage = Convert.ToInt32(page);
            ItemDisplay<Notification> notificationList = new ItemDisplay<Notification>();
            notificationList.CurrentPage = currentPage;

            List<Notification> notifications = NotificationDAO.Instance.GetNotificationList_Excel();

            notificationList.Paging(notifications, 5);

            if (notificationList.PageCount > 0)
            {
                if (notificationList.CurrentPage > notificationList.PageCount) notificationList.CurrentPage = notificationList.PageCount;
                if (notificationList.CurrentPage < 1) notificationList.CurrentPage = 1;
                if (notificationList.CurrentPage != notificationList.PageCount)
                    try
                    {
                        notificationList.Items = notificationList.Items.GetRange((notificationList.CurrentPage - 1) * notificationList.PageSize, notificationList.PageSize);
                    }
                    catch { }

                else
                    notificationList.Items = notificationList.Items.GetRange((notificationList.CurrentPage - 1) * notificationList.PageSize, notificationList.Items.Count % notificationList.PageSize == 0 ? notificationList.PageSize : notificationList.Items.Count % notificationList.PageSize);
            }
            return View("./Views/BNS/BNSHome.cshtml", notificationList);
        }

        public IActionResult NotificationDetail()
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();
            var currenId = Convert.ToInt32(CurrentID);

            var notification = NotificationDAO.Instance.GetNotificationModelbyId_Excel(currenId);
            return View("./Views/Shared/NotificationDetail.cshtml", notification);
        }



        public IActionResult ExportMemberToExcel()
        {
            List<Member> members = UserDAO.Instance.GetListUser_Excel();
            var stream = Function.Instance.ExportToExcel<Member>(members);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachThanhVien.xlsx");
        }

        public IActionResult AddMember()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String avt = urlQuery["avt"];
            avt = avt == null ? "default.jpg" : avt;
            return View("./Views/BNS/AddMember.cshtml", avt);
        }

        [HttpPost]
        public IActionResult UploadAvt(IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
        {
            string fileName = $"{hostingEnvironment.WebRootPath}/img/avt/{file.FileName}";
            // Dẩy file vào thư mục
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }
            // Gọi đến hàm đọc file gửi thằng đường dẫn file ta vừa lưu vào để đọc luôn
            TempData["avt"] = file.FileName;
            // Trả về dữ liệu
            return RedirectToAction("AddMember", "BNS", new {avt = file.FileName });
        }


        [HttpPost]
        public IActionResult AddMember(String LabID, String Name, String Sex, String Birthday, String Gen, String Unit, String Position)
        {
            String avt = TempData["avt"] == null ? "default.jpg" : TempData["avt"].ToString();
            var unit = Unit == null ? "Chưa có" : Unit;
            var position = Position == null ? "Chưa có" : Position;
            var newMember = new Member(LabID, avt, Name, Sex, Birthday, Gen, unit, position);
            UserDAO.Instance.AddMember(newMember);
            return RedirectToAction("Member");
        }

        public IActionResult DeleteMember()
        {
                var urlQuery = Request.HttpContext.Request.Query;
                String LabID_delete = urlQuery["LabID"];
                UserDAO.Instance.DeleteMember(LabID_delete);
            
            return RedirectToAction("Member");
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

            List<Member> members = UserDAO.Instance.GetListUser_Excel();
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

            return View("./Views/BNS/Member.cshtml", memberList);
        }



        [HttpPost]
        public IActionResult Member(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Member", "BNS", new { sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }


        public IActionResult MemberDetail()
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();

            var user = UserDAO.Instance.GetUserByID_Excel(CurrentID);
            return View("./Views/Shared/MemberDetail.cshtml", user);
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

            return View("./Views/BNS/Procedure.cshtml", procedureList);
        }

        [HttpPost]
        public IActionResult Procedure(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Procedure", "BNS", new { sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }
        public IActionResult ProcedureDetail()
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();
            var currenId = Convert.ToInt32(CurrentID);

            var procedure = ProcedureDAO.Instance.GetProcedureModel_Excel(currenId);
            return View("./Views/BNS/ProcedureDetail.cshtml", procedure);
        }
        public IActionResult AddProcedure()
        {
            return View("./Views/BNS/AddProcedure.cshtml");
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

        [HttpPost]
        public IActionResult EditProcedure(String Name, String Content, String Link)
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();
            var ID = Convert.ToInt32(CurrentID);

            var userSession = JsonConvert.DeserializeObject<UserLogin>(HttpContext.Session.GetString("LoginSession"));
            var unit = userSession.UserName; // unit
            var newProcedure = new Procedure(ID, Name, unit, Content.ToString(), Link);
            ProcedureDAO.Instance.EditProcedure(newProcedure);
            return RedirectToAction("Procedure");
        }

        public IActionResult DeleteProcedure()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String ProcedureId_delete = urlQuery["procedureID"];
            ProcedureDAO.Instance.DeleteProcedure(ProcedureId_delete);

            return RedirectToAction("Procedure");
        }
        public IActionResult ExportProcedureToExcel()
        {
            List<Procedure> procedures = ProcedureDAO.Instance.GetProcedureList_Excel();
            var stream = Function.Instance.ExportToExcel<Procedure>(procedures);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachQuytrinhBanNhansu.xlsx");
        }
    }
}
