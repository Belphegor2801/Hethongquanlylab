using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hethongquanlylab.Models;
using Hethongquanlylab.DAO;

namespace Hethongquanlylab.Controllers.Super.BanNhanSu
{
    public class BNSController : Controller
    {
        public IActionResult Index()
        {
            var notifications = NotificationDAO.Instance.GetNotificationList_Excel();
            return View("./Views/BNS/BNSHome.cshtml", notifications);
        }



        private List<Member> sortMember(List<Member> members, String sortOrder)
        {
            List<Member> memberList;
            switch (sortOrder)
            {
                case "id_desc":
                    memberList = members.OrderByDescending(s => Convert.ToInt32(s.LabID)).ToList();
                    break;
                case "Name":
                    memberList = members.OrderBy(s => s.Name.Split(" ").Last()).ToList();
                    break;
                case "name_desc":
                    memberList = members.OrderByDescending(s => s.Name.Split(" ").Last()).ToList();
                    break;
                case "Gen":
                    memberList = members.OrderBy(s => s.Gen).ToList();
                    break;
                case "gen_desc":
                    memberList = members.OrderByDescending(s => s.Gen).ToList();
                    break;
                case "Unit":
                    memberList = members.OrderBy(s => s.Unit).ToList();
                    break;
                case "unit_desc":
                    memberList = members.OrderByDescending(s => s.Unit).ToList();
                    break;

                default:
                    memberList = members.OrderBy(s => Convert.ToInt32(s.LabID)).ToList();
                    break;
            }
            return memberList;
        }


        public IActionResult Member(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            var members = UserDAO.Instance.GetListUser_Excel();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.GenSortParm = sortOrder == "Gen" ? "gen_desc" : "Gen";
            ViewBag.UnitSortParm = sortOrder == "Unit" ? "unit_desc" : "Unit";
            
            if (!String.IsNullOrEmpty(searchField))
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    switch (searchField)
                    {
                        case "Lab ID":
                            members= members.Where(s => s.LabID.Contains(searchString)).ToList();
                            break;
                        case "Name":
                            members = members.Where(s => s.Name.Contains(searchString)).ToList();
                            break;
                        case "Sex":
                            members = members.Where(s => s.Sex.Contains(searchString)).ToList();
                            break;
                        case "Birthday":
                            members = members.Where(s => s.Birthday.Contains(searchString)).ToList();
                            break;
                        case "Gen":
                            members = members.Where(s => s.Gen.Contains(searchString)).ToList();
                            break;
                        case "Unit":
                            members = members.Where(s => s.Unit.Contains(searchString)).ToList();
                            break;
                        case "Position":
                            members = members.Where(s => s.Position.Contains(searchString)).ToList();
                            break;
                        default:
                            members = members.Where(s => s.LabID.Contains(searchString)).ToList();
                            break;
                    }
                }
            }

            members = sortMember(members, sortOrder);

            var memberList = new MemberList(members);
            memberList.CurrentPage = currentPage;

            memberList.Members = memberList.Members.GetRange((memberList.CurrentPage - 1) * memberList.PageSize, memberList.PageSize);
            
            return View("./Views/BNS/Member.cshtml", memberList);
        }


        public IActionResult MemberDetail()
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();

            var user = UserDAO.Instance.GetUserByID_Excel(CurrentID);
            return View("./Views/BNS/MemberDetail.cshtml", user);
        }

        public IActionResult Procedure()
        {
            return View("./Views/BNS/Procedure.cshtml");
        }
    }
}
