using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hethongquanlylab.DAO;
using Hethongquanlylab.Models;
using Hethongquanlylab.Models.Login;

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
            var userSession = JsonConvert.DeserializeObject<UserLogin>(HttpContext.Session.GetString("LoginSession"));
            var user = UserDAO.Instance.GetUserByID_Excel("60");
            return View("./Views/User/Infor/Infor.cshtml", user);
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
