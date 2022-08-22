using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hethongquanlylab.Controllers.User
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View("./Views/User/UserHome.cshtml");
        }

        public IActionResult Infor()
        {
            return View("./Views/User/Infor/Infor.cshtml");
        }

        public IActionResult EditInfor()
        {
            return View("./Views/User/Infor/EditInfor.cshtml");
        }
        public IActionResult Training()
        {
            return View("./Views/User/Training.cshtml");
        }
    }
}
