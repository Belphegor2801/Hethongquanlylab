using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hethongquanlylab.Models;
using Hethongquanlylab.Models.Members;
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
                case "Birthday":
                    memberList = members.OrderBy(s => s.Birthday.Split("/").Last()).ToList();
                    break;
                case "birthday_desc":
                    memberList = members.OrderByDescending(s => s.Birthday.Split("/").Last()).ToList();
                    break;

                default:
                    memberList = members.OrderBy(s => Convert.ToInt32(s.LabID)).ToList();
                    break;
            }
            return memberList;
        }


        MemberList memberList = new MemberList();
        public IActionResult Member(String sortOrder, String searchString, String searchField, int currentPage = 1)
        {
            List<Member> members = UserDAO.Instance.GetListUser_Excel();
            memberList.Members = members;

            if (memberList.SortOrder != sortOrder)
            {
                memberList.SortOrder = sortOrder;
            }
            else
            {
            }

            if (memberList.CurrentSearchField != searchField){
                memberList.CurrentSearchField = searchField;
            }

            if (memberList.CurrentSearchString != searchString)
            {
                memberList.CurrentSearchString = searchString;
            }


            ViewBag.CurrentSort = sortOrder;
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.GenSortParm = sortOrder == "Gen" ? "gen_desc" : "Gen";
            ViewBag.UnitSortParm = sortOrder == "Unit" ? "unit_desc" : "Unit";
            
            if (!String.IsNullOrEmpty(memberList.CurrentSearchField))
            {
                if (!String.IsNullOrEmpty(memberList.CurrentSearchString))
                {
                    switch (memberList.CurrentSearchString)
                    {
                        case "Lab ID":
                            members= members.Where(s => s.LabID.Contains(memberList.CurrentSearchString)).ToList();
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

            members = sortMember(members, memberList.SortOrder);

            memberList.rePaging(members);
            
            memberList.CurrentPage = currentPage;

            if (memberList.PageCount > 0)
            {
            if (currentPage != memberList.PageCount)
                memberList.Members = memberList.Members.GetRange((memberList.CurrentPage - 1) * memberList.PageSize, memberList.PageSize);
            else
                memberList.Members = memberList.Members.GetRange((memberList.CurrentPage - 1) * memberList.PageSize, memberList.Members.Count % memberList.PageSize == 0? memberList.PageSize: memberList.Members.Count % memberList.PageSize);
            }

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
