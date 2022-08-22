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
            return View("./Views/Shared/Login/Login.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(String accName, String pass)
        {
            if (accName == "User")
                return RedirectToAction("Index", "User");
            else 
                return View("./Views/Shared/Login/Login.cshtml");
        }

        //
        public IActionResult ChangeToChangePassword()
        {
            return RedirectToAction("ChangePassword", "Login");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View("./Views/Shared/Login/ChangePassword.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangetPassword(String accName, String pass)
        {
            return RedirectToAction("Index", "User");
        }

        //
        public IActionResult ChangeToForgotPassword()
        {
            return RedirectToAction("ForgotPassword", "Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View("./Views/Shared/Login/ForgotPassword.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(String accName, String pass)
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
