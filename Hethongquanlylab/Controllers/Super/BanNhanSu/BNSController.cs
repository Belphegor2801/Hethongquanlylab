using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return View("./Views/BNS/Member.cshtml");
        }

        public IActionResult Procedure()
        {
            return View("./Views/BNS/Procedure.cshtml");
        }
    }
}
