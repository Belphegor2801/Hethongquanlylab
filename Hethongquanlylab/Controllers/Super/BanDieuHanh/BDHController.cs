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
        string unit = "Ban Điều Hành";
        //// Begin: Trang chủ
        /// Trang chủ
        public IActionResult Index()
        {
            String page;
            var urlQuery = Request.HttpContext.Request.Query;
            page = urlQuery["page"]; // Lấy trang thông báo
            var notificationList = Function.Instance.getNotifications(page);

            return View("./Views/BDH/BDHHome.cshtml", notificationList);
        }

        /// Thông tin chi tiết thông báo
        public IActionResult NotificationDetail()
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();
            var currenId = Convert.ToInt32(CurrentID); // Url: .../NotificationDetail/{ID}

            var notification = NotificationDAO.Instance.GetNotificationModelbyId_Excel(currenId);

            return View("./Views/Shared/NotificationDetail.cshtml", notification);
        }
        //// End: Trang chủ

        //// Begin Thông tin thành viên
        /// Bảng nhân sự
        public IActionResult Member()
        {
            // Khởi tạo
            String field;
            String sortOrder;
            String searchField;
            String searchString;
            String page;

            /// Lấy query, không có => đặt mặc định
            var urlQuery = Request.HttpContext.Request.Query; // Url: .../Member?Sort={sortOrder}&searchField={searchField}...
            field = urlQuery["field"];
            sortOrder = urlQuery["sort"];
            searchField = urlQuery["searchField"];
            searchString = urlQuery["SearchString"];
            page = urlQuery["page"];

            field = field == null ? "All" : field;
            sortOrder = sortOrder == null ? "LabID" : sortOrder; ;
            searchField = searchField == null ? "LabID" : searchField;
            searchString = searchString == null ? "" : searchString;
            page = page == null ? "1" : page;
            int currentPage = Convert.ToInt32(page);

            /// Khởi tạo ItemDisplay<>
            ItemDisplay<Member> memberList = new ItemDisplay<Member>();
            memberList.Field = field;
            memberList.SortOrder = sortOrder;
            memberList.CurrentSearchField = searchField;
            memberList.CurrentSearchString = searchString;
            memberList.CurrentPage = currentPage;

            List<Member> members;
            if (memberList.Field == "All")
                members = UserDAO.Instance.GetListUser_Excel();
            else if (memberList.Field == "PT")
                members = UserDAO.Instance.GetListUser_Excel("PT");
            else if (memberList.Field == "LT")
                members = UserDAO.Instance.GetListUser_Excel("LT");
            else if (memberList.Field == "BDH")
                members = UserDAO.Instance.GetListUser_Excel(unit);
            else
                members = UserDAO.Instance.GetListUser_Excel();


            members = Function.Instance.searchItems(members, memberList); // Tìm kiếm
            members = Function.Instance.sortItems(members, memberList.SortOrder); // Sắp xếp

            // Lấy danh sách items trong trang hiện tại
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
            //

            memberList.SessionVar = unit; // SessionVar => Để Section phần Header
            return View("./Views/BDH/Members/Member.cshtml", memberList);
        }

        [HttpPost]
        public IActionResult Member(String Field, String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Member", "BDH", new { field = Field, sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }

        /// Xuất file Excel
        public IActionResult ExportMemberToExcel()
        {
            List<Member> members = UserDAO.Instance.GetListUser_Excel();
            var stream = Function.Instance.ExportToExcel<Member>(members);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachThanhVien.xlsx");
        }

        // Thêm thành viên
        public IActionResult AddMember()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String avt = urlQuery["avt"];
            avt = avt == null ? "default.jpg" : avt; // Đặt avt mặc định nếu không up avt lên
            return View("./Views/BDH/Members/AddMember.cshtml", new List<string>() { unit, avt });
        }

        // Upload avt: trong Thêm thành viên
        [HttpPost]
        public IActionResult UploadAvt(string var, string key, IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
        {
            string fileName = $"{hostingEnvironment.WebRootPath}/img/avt/{file.FileName}";
            // Dẩy file vào thư mục
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }
            TempData["avt"] = file.FileName; // Lưu tên vào TempData => Lưu vào Excel
            if (var == "edit")
                return RedirectToAction("EditMember", new { avt = file.FileName, Key = key });
            else
            {
                return RedirectToAction("AddMember", new { avt = file.FileName });
            }

        }

        [HttpPost]
        public IActionResult AddMember(String LabID, String Name, String Sex, String Birthday, String Gen, String Specicalization, String University, String Phone, String Email, String Address, String Unit, String Position, bool IsLT, bool IsPassPTBT)
        {
            String avt = TempData["avt"] == null ? "default.jpg" : TempData["avt"].ToString();
            var unit = Unit == null ? "Chưa có" : Unit;
            var position = Position == null ? "Chưa có" : Position;
            var phone = Phone == null ? "N/A" : Phone;
            var email = Email == null ? "N/A" : Email;
            var address = Address == null ? "N/A" : Address;
            var specializaion = Specicalization == null ? "N/A" : Specicalization;
            var university = University == null ? "N/A" : University;
            var newMember = new Member(LabID, avt, Name, Sex, Birthday, Gen, phone, email, address, specializaion, university, unit, position, IsLT, IsPassPTBT);
            UserDAO.Instance.AddMember(newMember);
            return RedirectToAction("Member");
        }
        // End: thêm thành viên

        // Xóa thành viên
        public IActionResult DeleteMember()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String Key_delete = urlQuery["Key"]; // Url: .../DeteleMeber?Key={key}
            UserDAO.Instance.DeleteMember(Key_delete);

            return RedirectToAction("Member");
        }

        // Thông tin chi tiết thành viên: đưa đến 1 trang CV riêng ở tab mới
        public IActionResult MemberCV()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String CurrentID = urlQuery["Key"]; // Url: .../DeteleMeber?LabID={LabID}

            var user = UserDAO.Instance.GetUserByID_Excel(CurrentID);
            return View("./Views/Shared/MemberDetail.cshtml", user);
        }

        // Chỉnh sửa thông tin thành viên
        public IActionResult EditMember()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String CurrentID = urlQuery["Key"]; // Url: .../DeteleMeber?Key={key}
            String avt = urlQuery["avt"];

            var member = UserDAO.Instance.GetUserByID_Excel(CurrentID);
            if (avt != null) member.Avt = avt;
            var item = new ItemDetail<Member>(member, unit);
            return View("./Views/BDH/Members/EditMember.cshtml", item);
        }

        [HttpPost]
        public IActionResult EditMember(String Key, String LabID, String Name, String Sex, String Birthday, String Gen, String Specicalization, String University, String Phone, String Email, String Address, String Unit, String Position, bool IsLT, bool IsPassPTBT)
        {
            String avt = TempData["avt"] == null ? "default.jpg" : TempData["avt"].ToString();
            var unit = Unit == null ? "Chưa có" : Unit;
            var position = Position == null ? "Chưa có" : Position;
            var phone = Phone == null ? "N/A" : Phone;
            var email = Email == null ? "email@gmail.com" : Email;
            var address = Address == null ? "N/A" : Address;
            var specializaion = Specicalization == null ? "N/A" : Specicalization;
            var university = University == null ? "N/A" : University;
            var newMember = new Member(LabID, avt, Name, Sex, Birthday, Gen, phone, email, address, specializaion, university, unit, position, IsLT, IsPassPTBT, Key);
            UserDAO.Instance.EditMember(newMember);
            return RedirectToAction("Member");
        }

        //// End: Thông tin thành viên

        //// Begin: Thông tin quy trình
        /// Bảng quy trình
        public IActionResult Procedure()
        {
            String field;
            String sortOrder;
            String searchField;
            String searchString;
            String page;

            var urlQuery = Request.HttpContext.Request.Query;
            field = urlQuery["field"];
            sortOrder = urlQuery["sort"];
            searchField = urlQuery["searchField"];
            searchString = urlQuery["SearchString"];
            page = urlQuery["page"];

            field = field == null ? "All" : field;
            sortOrder = sortOrder == null ? "ID" : sortOrder;
            searchField = searchField == null ? "ID" : searchField;
            searchString = searchString == null ? "" : searchString;
            page = page == null ? "1" : page;
            int currentPage = Convert.ToInt32(page);


            ItemDisplay<Procedure> procedureList = new ItemDisplay<Procedure>();
            procedureList.Field = field;
            procedureList.SortOrder = sortOrder;
            procedureList.CurrentSearchField = searchField;
            procedureList.CurrentSearchString = searchString;
            procedureList.CurrentPage = currentPage;

            List<Procedure> procedures;
            if (procedureList.Field == "All")
                procedures = ProcedureDAO.Instance.GetProcedureList_Excel("Ban Điều Hành duyệt");
            else if (procedureList.Field == "BDH")
                procedures = ProcedureDAO.Instance.GetProcedureList_Excel(unit);
            else
                procedures = ProcedureDAO.Instance.GetProcedureList_Excel("Ban Điều Hành duyệt");
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

            procedureList.SessionVar = unit;
            return View("./Views/BDH/Procedures/Procedure.cshtml", procedureList);
        }

        [HttpPost]
        public IActionResult Procedure(String Field, String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Procedure", "BDH", new { field = Field, sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }

        // Chi tiết quy trình
        public IActionResult ProcedureDetail()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String ID = urlQuery["procedureID"];
            String Field = urlQuery["field"];

            Procedure procedure;
            if (Field == "BDH")
            {
                procedure = ProcedureDAO.Instance.GetProcedureModel_Excel(unit, ID);
            }
            else
            {
                procedure = ProcedureDAO.Instance.GetProcedureModel_Excel("Ban Điều Hành duyệt", ID);
            }

            var item = new ItemDetail<Procedure>(procedure, unit);
            item.FieldVar = Field;
            return View("./Views/BDH/Procedures/ProcedureDetail.cshtml", item);
        }

        // Chỉnh sửa quy trình
        [HttpPost]
        public IActionResult EditProcedure(String Name, String Content, String Link, String SubID, String IsSendToApproval)
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var ID = urlPath.ToString().Split('/').Last();
            var newProcedure = new Procedure(Name, unit, Content.ToString(), Link, ID, SubID); // Khởi tạo trạng thái mặc định quy trình: Status: Chưa duyệt

            if (IsSendToApproval == "y") // Nếu người dùng nhấn "Lưu và gửi duyệt"
            {
                newProcedure.Status = "Chờ duyệt";
                ProcedureDAO.Instance.EditProcedure(unit, newProcedure);
                ProcedureDAO.Instance.SendToApproval("Ban Điều Hành duyệt", newProcedure);
                ViewData["msg"] = Function.Instance.SendEmail("Duyệt quy trình", "Bạn có quy trình cần duyệt"); // Gửi mail và trả về thông báo
            }
            else
            {
                ProcedureDAO.Instance.EditProcedure(unit, newProcedure);
            }
            return RedirectToAction("Procedure", new { field = "BDH" });
        }

        // Thêm quy trình
        public IActionResult AddProcedure()
        {
            return View("./Views/BDH/Procedures/AddProcedure.cshtml", unit);
        }

        [HttpPost]
        public IActionResult AddProcedure(String Name, String Content, String Link, String IsSendToApproval)
        {
            var newProcedure = new Procedure(Name, unit, Content.ToString(), Link);

            if (IsSendToApproval == "y")
            {
                newProcedure.Status = "Chờ duyệt";
                ProcedureDAO.Instance.AddProcedure(unit, newProcedure);
                ProcedureDAO.Instance.SendToApproval("Ban Điều Hành duyệt", newProcedure);
                ViewData["msg"] = Function.Instance.SendEmail("Duyệt quy trình", "Bạn có quy trình cần duyệt");
            }
            else
            {
                ProcedureDAO.Instance.AddProcedure(unit, newProcedure);
            }

            return RedirectToAction("Procedure", new { field = "BDH" });
        }

        // Xóa quy trình
        public IActionResult DeleteProcedure()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String ProcedureId_delete = urlQuery["procedureID"];

            ProcedureDAO.Instance.DeleteProcedure(unit, ProcedureId_delete);

            return RedirectToAction("Procedure", new { field = "BDH" });
        }

        // Xuất file Excel Quy trình * Chưa xong
        public IActionResult ExportProcedureToExcel()
        {
            List<Procedure> procedures = ProcedureDAO.Instance.GetProcedureList_Excel(unit);
            var stream = Function.Instance.ExportToExcel<Procedure>(procedures);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Danh sách quy trình " + unit + ".xlsx");
        }

        // Gửi duyệt quy trình
        public IActionResult SendProceduresToApproval()
        {
            String sortOrder;
            String searchField;
            String searchString;

            var urlQuery = Request.HttpContext.Request.Query;
            sortOrder = urlQuery["sort"];
            searchField = urlQuery["searchField"];
            searchString = urlQuery["SearchString"];

            sortOrder = sortOrder == null ? "ID" : sortOrder; ;
            searchField = searchField == null ? "ID" : searchField;
            searchString = searchString == null ? "" : searchString;

            ItemDisplay<Procedure> procedureList = new ItemDisplay<Procedure>();
            procedureList.SortOrder = sortOrder;
            procedureList.CurrentSearchField = searchField;
            procedureList.CurrentSearchString = searchString;

            List<Procedure> procedures = ProcedureDAO.Instance.GetProcedureList_Excel(unit);
            procedures = Function.Instance.searchItems(procedures, procedureList);
            procedures = Function.Instance.sortItems(procedures, procedureList.SortOrder);
            procedureList.Items = procedures;
            procedureList.SessionVar = unit;

            return View("./Views/BDH/Procedures/SendProceduresToApproval.cshtml", procedureList);
        }

        [HttpPost]
        public IActionResult SendProceduresToApproval(String sortOrder, String searchString, String searchField, string isSendToApproval, string SendVar)
        {
            TempData["Sendvar"] = SendVar;
            if (isSendToApproval == "y")
            {
                int i = 0;
                // SendVar: 1:1-2:1-3:on-4:on-5:on-7:on-8:on-9:on-10:on-11:on-12:on-13:on-14:on-15:on- (ID:var[1: checked; on: unchecked])
                foreach (string item in SendVar.Split("-"))
                {
                    if (item.Split(":").Last() == "1")
                    {
                        i++;
                        Procedure procedure = ProcedureDAO.Instance.GetProcedureModel_Excel(unit, item.Split(":").First());
                        procedure.V1 = false;
                        procedure.V2 = false;
                        procedure.V3 = false;
                        procedure.Status = "Chờ duyệt";
                        ProcedureDAO.Instance.EditProcedure(unit, procedure);
                        ProcedureDAO.Instance.SendToApproval("Ban Điều Hành duyệt", procedure);
                    }
                }
                if (i > 0)
                {
                    ViewData["msg"] = Function.Instance.SendEmail("Duyệt quy trình", "Bạn có " + i.ToString() + " quy trình cần duyệt");
                }
                return RedirectToAction("Procedure", new { field = "BDH" });
            }
            return RedirectToAction("SendProceduresToApproval", "BDH", new { sort = sortOrder, searchField = searchField, searchString = searchString });
        }
        //// End: Thông tin quy trình

        [HttpPost]
        public IActionResult FeedbackProcedure(String feedback, String IsApproval)
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var ID = urlPath.ToString().Split('/').Last();
            Procedure newProcedure = ProcedureDAO.Instance.GetProcedureModel_Excel("Ban Điều Hành duyệt", ID);
            if (IsApproval == "y")
            {
                ProcedureDAO.Instance.ApprovalProcedure(unit, newProcedure, feedback);
            }
            else
            {
                ProcedureDAO.Instance.ReturnProcedure(unit, newProcedure, feedback);
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
    }
}
