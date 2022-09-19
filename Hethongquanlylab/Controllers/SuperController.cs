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

namespace Hethongquanlylab.Controllers
{
    public class SuperController : Controller
    {
        public string unit { get; set; }
        public string unitVar { get; set; }

        //// Begin: Trang chủ
        /// Trang chủ
        public IActionResult Index()
        {
            String page;
            var urlQuery = Request.HttpContext.Request.Query;
            page = urlQuery["page"]; // Lấy trang thông báo
            var notificationList = Function.Instance.getNotifications(page);
            string formLink = LinkDAO.Instance.GetLink("Lịch làm việc");
            notificationList.Link = formLink;
            return View(String.Format("./Views/{0}/{0}Home.cshtml", unitVar), notificationList);
        }

        [HttpPost]
        public IActionResult Index(int currentPage = 1)
        {
            return RedirectToAction("Index", new { page = currentPage });
        }

        /// Thông tin chi tiết thông báo
        public IActionResult NotificationDetail()
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();
            var currenId = Convert.ToInt32(CurrentID); // Url: .../NotificationDetail/{ID}

            var notification = NotificationDAO.Instance.GetNotificationModelbyId_Excel(currenId);
            var item = new ItemDetail<Notification>(notification, unit);
            return View("./Views/Shared/NotificationDetail.cshtml", item);
        }
        //// End: Trang chủ

        public IActionResult EditLinks()
        {
            var links = LinkDAO.Instance.GetLinkList();
            var linkList = new ItemDisplay<Link>();
            linkList.Items = links;
            linkList.SessionVar = unit;
            return View(String.Format("./Views/{0}/Links/EditLinks.cshtml", unitVar), linkList);
        }

        [HttpPost]
        public IActionResult EditLinks(string Link_bieumau, string Link_lich)
        {
            LinkDAO.Instance.EditLink("Biểu mẫu", Link_bieumau);
            LinkDAO.Instance.EditLink("Lịch làm việc", Link_lich);

            return RedirectToAction("EditLinks");
        }

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

            if (unit == "Ban Nhân Sự")
            {
                field = field == null ? "All" : field;
            }
            else
            {
                field = field == null ? unitVar : field;
            }
            
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
            else
                members = UserDAO.Instance.GetListUser_Excel(unit);

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
            return View(String.Format("./Views/{0}/Members/Member.cshtml", unitVar), memberList);
        }

        [HttpPost]
        public IActionResult Member(String Field, String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Member", new { field = Field, sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }

        /// Xuất file Excel
        public IActionResult ExportMemberToExcel()
        {
            var urlQuery = Request.HttpContext.Request.Query; // Url: .../Member?Sort={sortOrder}&searchField={searchField}...
            string exportVar = urlQuery["exportVar"];
            exportVar = exportVar == null ? unitVar : exportVar;

            List<Member> members;
            if (exportVar == "All")
                members = UserDAO.Instance.GetListUser_Excel();
            else if (exportVar == "PT")
                members = UserDAO.Instance.GetListUser_Excel("PT");
            else if (exportVar == "LT")
                members = UserDAO.Instance.GetListUser_Excel("LT");
            else
                members = UserDAO.Instance.GetListUser_Excel(unit);

            var stream = Function.Instance.ExportToExcel<Member>(members);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachThanhVien-" + exportVar +".xlsx");
        }

        // Thêm thành viên
        public virtual IActionResult AddMember()
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

            if (unit == "Ban Nhân Sự")
            {
                field = field == null ? "All" : field;
            }
            else
            {
                field = field == null ? unitVar : field;
            }

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
            return View(String.Format("./Views/{0}/Members/AddMember.cshtml", unitVar), memberList);
        }

        // Upload avt: trong Thêm thành viên
        [HttpPost]
        public virtual IActionResult UploadAvt(string var, string key, IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
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
        public virtual IActionResult AddMember(String sortOrder, String searchString, String searchField, string IsAdd, string MembersVar, String Key, String LabID, String Name, String Sex, String Birthday, String Gen, String Phone, String Email, String Address, String Specicalization, String University, String Unit, String Position, bool IsLT, bool IsPassPTBT)
        {
            if (IsAdd == "y")
            {
                int i = 0;
                // SendVar: 1:1-2:1-3:on-4:on-5:on-7:on-8:on-9:on-10:on-11:on-12:on-13:on-14:on-15:on- (ID:var[1: checked; on: unchecked])
                foreach (string item in MembersVar.Split("-"))
                {
                    if (item.Split(":").Last() == "1")
                    {
                        i++;
                        Member member = UserDAO.Instance.GetUserByID_Excel(item.Split(":").First());
                        if (member.Unit == "Chưa có") member.Unit = unit;
                        else member.Unit = member.Unit + unit;
                        UserDAO.Instance.EditMember(member);
                    }
                }
                if (i > 0)
                {
                    ViewData["msg"] = Function.Instance.SendEmail("Duyệt quy trình", "Bạn có " + i.ToString() + " quy trình cần duyệt");
                }
                return RedirectToAction("Member");
            }
            return RedirectToAction("AddMember", new { sort = sortOrder, searchField = searchField, searchString = searchString });
        }
        // End: thêm thành viên

        // Xóa thành viên
        public IActionResult DeleteMember()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String Key_delete = urlQuery["Key"]; // Url: .../DeteleMeber?LabID={LabID}
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
            return View(String.Format("./Views/{0}/Members/EditMember.cshtml", unitVar), item);
        }

        [HttpPost]
        public IActionResult EditMember(String Key, String LabID, String Name, String Sex, String Birthday, String Gen, String Phone, String Email, String Address, String Specicalization, String University, String Unit, String Position, bool IsLT, bool IsPassPTBT)
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

            if ((unitVar == "BDH") || (unitVar == "BCV") || (unitVar == "NSL"))
            {
                field = field == null ? "All" : field;
            }
            else
            {
                field = field == null ? unitVar : field;
            }
            
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
                procedures = ProcedureDAO.Instance.GetProcedureList_Excel(String.Format("{0} duyệt", unit));
            else
                procedures = ProcedureDAO.Instance.GetProcedureList_Excel(unit);

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
            return View(String.Format("./Views/{0}/Procedures/Procedure.cshtml", unitVar), procedureList);
        }

        [HttpPost]
        public IActionResult Procedure(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Procedure", new { sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }

        // Chi tiết quy trình
        public IActionResult ProcedureDetail()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String ID = urlQuery["procedureID"];
            var procedure = ProcedureDAO.Instance.GetProcedureModel_Excel(unit, ID);
            var item = new ItemDetail<Procedure>(procedure, unit);
            return View(String.Format("./Views/{0}/Procedures/ProcedureDetail.cshtml", unitVar), item);
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
                newProcedure.V1 = false;
                newProcedure.V2 = false;
                newProcedure.V3 = false;
                newProcedure.Status = "Chờ duyệt";
                ProcedureDAO.Instance.EditProcedure(unit, newProcedure);
                if (unit == "Ban Điều Hành")
                {
                    ProcedureDAO.Instance.SendToApproval("Ban Cố Vấn duyệt", newProcedure);
                }
                else if (unit == "Ban Cố Vấn")
                {
                    ProcedureDAO.Instance.SendToApproval("Nhà Sáng Lập duyệt", newProcedure);
                }
                else
                {
                    ProcedureDAO.Instance.SendToApproval("Ban Điều Hành duyệt", newProcedure);
                }
                
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
            return View(String.Format("./Views/{0}/Procedures/AddProcedure.cshtml", unitVar), unit);
        }

        [HttpPost]
        public IActionResult AddProcedure(String Name, String Content, String Link, String IsSendToApproval)
        {
            var newProcedure = new Procedure(Name, unit, Content.ToString(), Link);

            if (IsSendToApproval == "y")
            {
                newProcedure.Status = "Chờ duyệt";
                ProcedureDAO.Instance.AddProcedure(unit, newProcedure);
                if (unit == "Ban Điều Hành")
                {
                    ProcedureDAO.Instance.SendToApproval("Ban Cố Vấn duyệt", newProcedure);
                }
                else if (unit == "Ban Cố Vấn")
                {
                    ProcedureDAO.Instance.SendToApproval("Nhà Sáng Lập duyệt", newProcedure);
                }
                else
                {
                    ProcedureDAO.Instance.SendToApproval("Ban Điều Hành duyệt", newProcedure);
                }
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
            var urlQuery = Request.HttpContext.Request.Query; // Url: .../Member?Sort={sortOrder}&searchField={searchField}...
            string exportVar = urlQuery["exportVar"];
            exportVar = exportVar == null ? unitVar : exportVar;

            List<Procedure> procedures;
            if (exportVar == "All")
                procedures = ProcedureDAO.Instance.GetProcedureList_Excel("All");
            else
                procedures = ProcedureDAO.Instance.GetProcedureList_Excel(unit);

            var stream = Function.Instance.ExportToExcel<Procedure>(procedures);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Danh sách quy trình " + exportVar + ".xlsx");
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
            procedures = procedures.Where(s => !s.Status.Contains("đã duyệt")).ToList();
            procedures = procedures.Where(s => !s.Status.Contains("Chờ duyệt")).ToList();
            procedureList.Items = procedures;
            procedureList.SessionVar = unit;

            return View(String.Format("./Views/{0}/Procedures/SendProceduresToApproval.cshtml", unitVar), procedureList);
        }

        [HttpPost]
        public IActionResult SendProceduresToApproval(String sortOrder, String searchString, String searchField, string isSendToApproval, string SendVar)
        {
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
                        if (unit == "Ban Điều Hành")
                        {
                            ProcedureDAO.Instance.SendToApproval("Ban Cố Vấn duyệt", procedure);
                        }
                        else if (unit == "Ban Cố Vấn")
                        {
                            ProcedureDAO.Instance.SendToApproval("Nhà Sáng Lập duyệt", procedure);
                        }
                        else
                        {
                            ProcedureDAO.Instance.SendToApproval("Ban Điều Hành duyệt", procedure);
                        }
                    }
                }
                if (i > 0)
                {
                    ViewData["msg"] = Function.Instance.SendEmail("Duyệt quy trình", "Bạn có " + i.ToString() + " quy trình cần duyệt");
                }
                return RedirectToAction("Procedure");
            }
            return RedirectToAction("SendProceduresToApproval", new { sort = sortOrder, searchField = searchField, searchString = searchString });
        }
        //// End: Thông tin quy trình
        ///

        public IActionResult Training()
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

            if (unit == "Ban Đào Tạo")
                field = field == null ? "All" : field;
            else
                field = field == null ? unitVar : field;

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


            List<Training> trainings;
            if (field == unitVar)
                trainings = TrainingDAO.Instance.GetTrainingList_Excel(unitVar);
            else
                trainings = TrainingDAO.Instance.GetTrainingList_Excel("All");

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

            trainingList.SessionVar = unit;
            return View(String.Format("./Views/{0}/Trainings/Training.cshtml", unitVar), trainingList);
        }
        [HttpPost]
        public IActionResult Training(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Training", new { sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }
        public IActionResult TrainingDetail()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            string ID = urlQuery["trainingID"];
            Training training;
            if (unit == "Ban Đào Tạo")
            {
                training = TrainingDAO.Instance.GetTrainingModelbyId_Excel("All", ID);
            }
            else
            {
                training = TrainingDAO.Instance.GetTrainingModelbyId_Excel(unit, ID);
            }
            var item = new ItemDetail<Training>(training, unit);
            return View(String.Format("./Views/{0}/Trainings/TrainingDetail.cshtml", unitVar), item);
        }

        [HttpPost]
        public IActionResult EditTraining(String Name, String Content, String Link, String SubID)
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var ID = urlPath.ToString().Split('/').Last();
            var training = new Training(Name, unit, Content.ToString(), Link, ID, SubID); // Khởi tạo trạng thái mặc định quy trình: Status: Chưa duyệt
            if (unit == "Ban Đào Tạo")
            {
                TrainingDAO.Instance.EditTraing("All", training);
            }
            else
            {
                TrainingDAO.Instance.EditTraing(unit, training);
            }

            return RedirectToAction("Training");
        }

        public IActionResult AddTraining()
        {
            return View(String.Format("./Views/{0}/Trainings/AddTraining.cshtml", unitVar), unit);
        }

        [HttpPost]
        public IActionResult AddTraining(String Name, String Content, String Link, String IsSendToApproval)
        {
            var training = new Training(Name, unit, Content.ToString(), Link);

            if (unit == "Ban Đào Tạo")
            {
                TrainingDAO.Instance.AddTraining("All", training);
            }
            else
            {
                TrainingDAO.Instance.AddTraining(unit, training);
            }
            return RedirectToAction("Training");
        }


        public IActionResult ExportTrainingToExcel()
        {
            var urlQuery = Request.HttpContext.Request.Query; // Url: .../Member?Sort={sortOrder}&searchField={searchField}...
            string exportVar = urlQuery["exportVar"];
            exportVar = exportVar == null ? unitVar : exportVar;
            List<Training> trainings;
            if (exportVar == unitVar)
                trainings = TrainingDAO.Instance.GetTrainingList_Excel(unitVar);
            else
                trainings = TrainingDAO.Instance.GetTrainingList_Excel("All");

            List<Training> training = TrainingDAO.Instance.GetTrainingList_Excel("All");
            var stream = Function.Instance.ExportToExcel<Training>(training);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Danh sách bài đào tạo.xlsx");
        }
        
        public IActionResult Project()
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

            if (unit == "Ban Đào Tạo")
                field = field == null ? "All" : field;
            else
                field = field == null ? unitVar : field;

            sortOrder = sortOrder == null ? "ID" : sortOrder; ;
            searchField = searchField == null ? "ID" : searchField;
            searchString = searchString == null ? "" : searchString;
            page = page == null ? "1" : page;
            int currentPage = Convert.ToInt32(page);

            ItemDisplay<Project> projectList = new ItemDisplay<Project>();
            projectList.SortOrder = sortOrder;
            projectList.CurrentSearchField = searchField;
            projectList.CurrentSearchString = searchString;
            projectList.CurrentPage = currentPage;


            List<Project> projects;
            if (field == unitVar)
                projects = ProjectDAO.Instance.GetProjectList_Excel(unitVar);
            else
                projects = ProjectDAO.Instance.GetProjectList_Excel("All");

            projects = Function.Instance.searchItems(projects, projectList);
            projects = Function.Instance.sortItems(projects, projectList.SortOrder);

            projectList.Paging(projects, 10);

            if (projectList.PageCount > 0)
            {
                if (projectList.CurrentPage > projectList.PageCount) projectList.CurrentPage = projectList.PageCount;
                if (projectList.CurrentPage < 1) projectList.CurrentPage = 1;
                if (projectList.CurrentPage != projectList.PageCount)
                    try
                    {
                        projectList.Items = projectList.Items.GetRange((projectList.CurrentPage - 1) * projectList.PageSize, projectList.PageSize);
                    }
                    catch { }

                else
                    projectList.Items = projectList.Items.GetRange((projectList.CurrentPage - 1) * projectList.PageSize, projectList.Items.Count % projectList.PageSize == 0 ? projectList.PageSize : projectList.Items.Count % projectList.PageSize);
            }

            projectList.SessionVar = unit;
            return View(String.Format("./Views/{0}/Projects/Project.cshtml", unitVar), projectList);
        }
        [HttpPost]
        public IActionResult Project(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Project", new { sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }
        public IActionResult ProjectDetail()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            string ID = urlQuery["ProjectID"];
            Project project;
            if (unit == "Ban Đào Tạo")
            {
                project = ProjectDAO.Instance.GetProjectModelbyId_Excel("All", ID);
            }
            else
            {
                project = ProjectDAO.Instance.GetProjectModelbyId_Excel(unit, ID);
            }
            var item = new ItemDetail<Project>(project, unit);
            return View(String.Format("./Views/{0}/Projects/ProjectDetail.cshtml", unitVar), item);
        }

        [HttpPost]
        /*public IActionResult EditProject(String Name, String Content, String Link, String SubID)
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var ID = urlPath.ToString().Split('/').Last();
            var project = new Project(Name, unit, Content.ToString(), Link, ID, SubID); // Khởi tạo trạng thái mặc định quy trình: Status: Chưa duyệt
            if (unit == "Ban Đào Tạo")
            {
                ProjectDAO.Instance.EditProject("All", project);
            }
            else
            {
                ProjectDAO.Instance.EditProject(unit, project);
            }

            return RedirectToAction("Project");
        }
        */
        public IActionResult AddProject()
        {
            return View(String.Format("./Views/{0}/Projects/AddProject.cshtml", unitVar), unit);
        }

        [HttpPost]
        /*public IActionResult AddProject(String Name, String Content, String Link, String Status, String IsSendToApproval)
        {
            var project = new Project(Name, unit, Content.ToString(), Link);

            if (unit == "Ban Đào Tạo")
            {
                ProjectDAO.Instance.AddProject("All", project);
            }
            else
            {
                ProjectDAO.Instance.AddProject(unit, project);
            }
            return RedirectToAction("Project");
        }*/


        public IActionResult ExportProjectToExcel()
        {
            return View();
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
            itemList.SessionVar = unit;
            return View(String.Format("./Views/{0}/Notifications/Notification.cshtml", unitVar), itemList);
        }
        [HttpPost]
        public IActionResult Notification(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            return RedirectToAction("Notification", new { sort = sortOrder, searchField = searchField, searchString = searchString, page = currentPage });
        }

        public IActionResult ExportNotificationToExcel()
        {
            var urlQuery = Request.HttpContext.Request.Query; // Url: .../Member?Sort={sortOrder}&searchField={searchField}...
            string exportVar = urlQuery["exportVar"];
            exportVar = exportVar == null ? unitVar : exportVar;
            List<Notification> notifications;
            if (exportVar == unitVar)
                notifications = NotificationDAO.Instance.GetNotificationList_Excel();
            else
                notifications = NotificationDAO.Instance.GetNotificationList_Excel();

            var stream = Function.Instance.ExportToExcel<Notification>(notifications);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Danh sách thông báo.xlsx");
        }

        public IActionResult NotiDetail()
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();
            var currenId = Convert.ToInt32(CurrentID); // Url: .../NotificationDetail/{ID}

            var notification = NotificationDAO.Instance.GetNotificationModelbyId_Excel(currenId);
            var item = new ItemDetail<Notification>(notification, unit);
            return View(String.Format("./Views/{0}/Notifications/NotificationDetail.cshtml", unitVar), item);
        }

        public IActionResult AddNotification()
        {
            return View(String.Format("./Views/{0}/Notifications/AddNotification.cshtml", unitVar));
        }

        [HttpPost]
        public IActionResult AddNotification(String Title, String Content, String Date, String Link)
        {
            int ID = NotificationDAO.Instance.GetMaxID() + 1;
            var newNotification = new Notification(ID, Title, Content, unit, Date, Link);
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


    }
}
