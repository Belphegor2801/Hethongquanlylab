using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hethongquanlylab.DAO;
using Hethongquanlylab.Models;
using System.IO;
using OfficeOpenXml;

namespace Hethongquanlylab.Controllers.Super.BanDaoTao
{
    public class BDTController : Controller
    {
        public IActionResult Index()
        {
            var notifications = NotificationDAO.Instance.GetNotificationList_Excel();
            return View("./Views/BDT/BDTHome.cshtml", notifications);
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
                case "Unit":
                    members = members.OrderBy(s => s.Unit).ToList();
                    break;
                case "unit_desc":
                    members = members.OrderByDescending(s => s.Unit).ToList();
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

        private List<Member> searchMember(List<Member> members, MemberList memberList)
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
            String sortOrder = "LabID";
            String searchField = "LabID";
            String searchString = "";
            int page = 1;

            var urlQuery = Request.HttpContext.Request.Query;
            foreach (var attr in urlQuery.Keys)
            {
                if (attr == "sort") sortOrder = urlQuery[attr];
                if (attr == "searchField") searchField = urlQuery[attr];
                if (attr == "searchString") searchString = urlQuery[attr];
                if (attr == "page") page = Convert.ToInt32(urlQuery[attr]);
            }


            MemberList memberList = new MemberList();
            memberList.SortOrder = sortOrder;
            memberList.CurrentSearchField = searchField;
            memberList.CurrentSearchString = searchString;
            memberList.CurrentPage = page;



            List<Member> members = UserDAO.Instance.FindMemberbyUnit("PowerTeam Lập trình");
            members = searchMember(members, memberList);
            members = sortMember(members, memberList.SortOrder);


            memberList.Paging(members, 10);

            if (memberList.PageCount > 0)
            {
                if (memberList.CurrentPage != memberList.PageCount)
                    memberList.Members = memberList.Members.GetRange((memberList.CurrentPage - 1) * memberList.PageSize, memberList.PageSize);
                else
                    memberList.Members = memberList.Members.GetRange((memberList.CurrentPage - 1) * memberList.PageSize, memberList.Members.Count % memberList.PageSize == 0 ? memberList.PageSize : memberList.Members.Count % memberList.PageSize);
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
            return View("./Views/BDT/MemberDetail.cshtml", user);
        }

        public IActionResult Procedure()
        {
            var procedure = ProcedureDAO.Instance.GetProcedureList_Excel();
            return View("./Views/BDT/Procedure.cshtml", procedure);
        }
        public IActionResult AddProcedure()
        {
            return View("./Views/BDT/AddProcedure.cshtml");
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
        public IActionResult Notification()
        {
            var notification = NotificationDAO.Instance.GetNotificationListbyUnit("Ban Đào tạo");
            return View("./Views/BDT/Notification.cshtml", notification);
        }
        public IActionResult Project()
        {
            var project = ProjectDAO.Instance.GetProjectList_Excel();
            return View("./Views/BDT/Project.cshtml", project);
        }
        public IActionResult Training()
        {
            var training = TrainingDAO.Instance.GetTrainingList_Excel();
            return View("./Views/BDT/Training.cshtml", training);
        }
    }
}
