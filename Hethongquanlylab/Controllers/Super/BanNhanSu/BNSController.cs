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
        string unit = "Ban Nhân Sự";
        //// Begin: Trang chủ
        /// Trang chủ
        public IActionResult Index()
        {
            String page;
            var urlQuery = Request.HttpContext.Request.Query; 
            page = urlQuery["page"]; // Lấy trang thông báo
            var notificationList = Function.Instance.getNotifications(page);

            return View("./Views/BNS/BNSHome.cshtml", notificationList);
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
            else if (memberList.Field == "BNS")
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
            return View("./Views/BNS/Member.cshtml", memberList);
        }

        [HttpPost]
        public IActionResult Member(String Field, String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Member", "BNS", new { field = Field, sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
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
            return View("./Views/BNS/AddMember.cshtml", new List<string>() {unit, avt });
        }

        // Upload avt: trong Thêm thành viên
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
            TempData["avt"] = file.FileName; // Lưu tên vào TempData => Lưu vào Excel
            return RedirectToAction("AddMember", "BNS", new { avt = file.FileName });
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
        // End: thêm thành viên

        // Xóa thành viên
        public IActionResult DeleteMember()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String LabID_delete = urlQuery["LabID"]; // Url: .../DeteleMeber?LabID={LabID}
            UserDAO.Instance.DeleteMember(LabID_delete);

            return RedirectToAction("Member");
        }

        // Thông tin chi tiết thành viên: đưa đến 1 trang CV riêng ở tab mới
        public IActionResult MemberDetail()
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();

            var user = UserDAO.Instance.GetUserByID_Excel(CurrentID);
            return View("./Views/Shared/MemberDetail.cshtml", user);
        }
        //// End: Thông tin thành viên

        //// Begin: Thông tin quy trình
        /// Bảng quy trình
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

            List<Procedure> procedures = ProcedureDAO.Instance.GetProcedureList_Excel(unit);
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
            return View("./Views/BNS/Procedure.cshtml", procedureList);
        }

        [HttpPost]
        public IActionResult Procedure(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Procedure", "BNS", new { sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }
        
        // Chi tiết quy trình
        public IActionResult ProcedureDetail()
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();
            var currenId = Convert.ToInt32(CurrentID);

            var procedure = ProcedureDAO.Instance.GetProcedureModel_Excel(unit, currenId);
            return View("./Views/BNS/ProcedureDetail.cshtml", procedure);
        }

        // Chỉnh sửa quy trình
        [HttpPost]
        public IActionResult EditProcedure(String Name, String Content, String Link, String IsSendToApproval)
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();
            var ID = Convert.ToInt32(CurrentID);
            var newProcedure = new Procedure(ID, Name, unit, Content.ToString(), Link); // Khởi tạo trạng thái mặc định quy trình: Status: Chưa duyệt
            
            if (IsSendToApproval == "y") // Nếu người dùng nhấn "Lưu và gửi duyệt"
            {
                newProcedure.Status = "Chờ duyệt";
                ProcedureDAO.Instance.EditProcedure(unit, newProcedure);
                ProcedureDAO.Instance.SendToApproval(newProcedure);
                ViewData["msg"] = Function.Instance.SendEmail("Duyệt quy trình", "Bạn có quy trình cần duyệt"); // Gửi mail và trả về thông báo
            }
            else
            {
                ProcedureDAO.Instance.EditProcedure(unit, newProcedure);
            }
            return RedirectToAction("Procedure");
        }

        // Thêm quy trình
        public IActionResult AddProcedure()
        {

            return View("./Views/BNS/AddProcedure.cshtml", "BanNhanSu");
        }

        [HttpPost]
        public IActionResult AddProcedure(String Name, String Content, String Link, String IsSendToApproval)
        {
            int ID = ProcedureDAO.Instance.GetMaxID() + 1;
            var newProcedure = new Procedure(ID, Name, unit, Content.ToString(), Link);

            if (IsSendToApproval == "y")
            {
                newProcedure.Status = "Chờ duyệt";
                ProcedureDAO.Instance.AddProcedure(unit, newProcedure);
                ViewData["msg"] = Function.Instance.SendEmail("Duyệt quy trình", "Bạn có quy trình cần duyệt");
            }
            else
            {
                ProcedureDAO.Instance.AddProcedure(unit, newProcedure);
            }

            return RedirectToAction("Procedure");
        }

        // Xóa quy trình
        public IActionResult DeleteProcedure()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String ProcedureId_delete = urlQuery["procedureID"];

            ProcedureDAO.Instance.DeleteProcedure(unit, ProcedureId_delete);

            return RedirectToAction("Procedure");
        }

        // Xuất file Excel Quy trình * Chưa xong
        public IActionResult ExportProcedureToExcel()
        {
            List<Procedure> procedures = ProcedureDAO.Instance.GetProcedureList_Excel(unit);
            var stream = Function.Instance.ExportToExcel<Procedure>(procedures);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachQuytrinhBanNhansu.xlsx");
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

            return View("./Views/BNS/SendProceduresToApproval.cshtml", procedureList);
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
                        Procedure procedure = ProcedureDAO.Instance.GetProcedureModel_Excel(unit, Convert.ToInt32(item.Split(":").First()));
                        procedure.V1 = false;
                        procedure.V2 = false;
                        procedure.V3 = false;
                        procedure.Status = "Chờ duyệt";
                        ProcedureDAO.Instance.EditProcedure(unit, procedure);
                        ProcedureDAO.Instance.SendToApproval(procedure);
                    }
                }
                if (i > 0)
                {
                    ViewData["msg"] = Function.Instance.SendEmail("Duyệt quy trình", "Bạn có " + i.ToString() +" quy trình cần duyệt");
                }
                return RedirectToAction("Procedure");
            }
            return RedirectToAction("SendProceduresToApproval", "BNS", new { sort = sortOrder, searchField = searchField, searchString = searchString});
        }
        //// End: Thông tin quy trình
    }
}
