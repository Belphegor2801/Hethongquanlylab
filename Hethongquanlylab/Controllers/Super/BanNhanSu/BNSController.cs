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
            return View("./Views/BNS/BNSHome.cshtml");
        }


        public IActionResult Member()
        {
            var users = UserDAO.Instance.GetListUser_Excel();
            return View("./Views/BNS/Member.cshtml", users);
        }
        [HttpPost]
        public IActionResult Member(String ID)
        {
            List<Member> users = new List<Member>();
            if (UserDAO.Instance.GetUserByID_Excel(ID) != null)
                users.Add(UserDAO.Instance.GetUserByID_Excel(ID));
            return View("./Views/BNS/Member.cshtml", users);
        }



        public IActionResult Procedure()
        {
            return View("./Views/BNS/Procedure.cshtml");
        }
    }
}
