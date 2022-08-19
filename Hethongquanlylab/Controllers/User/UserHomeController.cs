using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hethongquanlylab.Controllers.User
{
    public class UserHomeController : Controller
    {
        public IActionResult Index()
        {
            return View("./Views/User/UserHome/Index.cshtml");
        }
    }
}
