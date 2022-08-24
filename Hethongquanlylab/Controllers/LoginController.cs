using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Hethongquanlylab.DAO;
using Hethongquanlylab.Models;

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
        public IActionResult Index(String accName = null, String pass = null)
        {
            if (accName == null) // Chưa nhập tên đăng nhập
            {
                TempData["msg"] = "Vui lòng nhập tên đăng nhập!";
                return View("./Views/Shared/Login/Login.cshtml");
            }
            Account user = AccountDAO.Instance.GetAccountbyUsername_Excel(accName);
            if (user == null)  // Không tìm thấy tên đăng nhập trong database
            {
                TempData["msg"] = "Tài khoản không tồn tại!";
                return View("./Views/Shared/Login/Login.cshtml");
            }
            // Tìm thấy tên đăng nhập trong database
            if (pass == null) // Chưa nhập mật khẩu
            {
                TempData["msg"] = "Vui lòng nhập mật khẩu";
                return View("./Views/Shared/Login/Login.cshtml");
            }
            else if (pass != user.Password) // Nhập sai mật khẩu
            {
                TempData["msg"] = "Bạn nhập sai mật khẩu!";
                return View("./Views/Shared/Login/Login.cshtml");
            }
            else // Đúng mật khẩu và chuyển hướng loại tài khoản
            {
                if (user.AccountType == "user")
                    return RedirectToAction("Index", "User");
                else if (user.AccountType == "super")
                    return RedirectToAction("Index", "Super");
                else if (user.AccountType == "admin")
                    return RedirectToAction("Index", "Admin");
                else
                    return View("./Views/Shared/Login/Login.cshtml");
            }
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
