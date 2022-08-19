using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hethongquanlylab.Controllers
{
    public class LoginController : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
            return View("./Views/Shared/Login.cshtml");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(String accName, String pass)
        {
            return RedirectToAction("Index", "UserHome");
        }
    }
}
