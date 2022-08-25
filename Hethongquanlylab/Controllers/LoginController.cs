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

namespace Hethongquanlylab.Controllers
{
    public class LoginController : Controller
    {

        // Index
        AccountLoginInput accountLoginInput = new AccountLoginInput();
        public IActionResult ChangeToLoginIndex() //Action đệm, tránh HttpPost
        {
            
            return RedirectToAction("Index", "Login");
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View("./Views/Shared/Login/Login.cshtml", accountLoginInput);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(AccountLoginInput input)
        {
            accountLoginInput = input; // Lưu thông tin người dùng nhập
            if (input.UserName == null) // Chưa nhập tên đăng nhập
            {
                TempData["msg"] = "Vui lòng nhập tên đăng nhập!";
                return View("./Views/Shared/Login/Login.cshtml", accountLoginInput);
            }
            Account user = AccountDAO.Instance.GetAccountbyUsername_Excel(input.UserName);
            if (user == null)  // Không tìm thấy tên đăng nhập trong database
            {
                TempData["msg"] = "Tài khoản không tồn tại!";
                return View("./Views/Shared/Login/Login.cshtml", accountLoginInput);
            }
            // Tìm thấy tên đăng nhập trong database
            if (input.Password == null) // Chưa nhập mật khẩu
            {
                TempData["msg"] = "Vui lòng nhập mật khẩu";
                return View("./Views/Shared/Login/Login.cshtml", accountLoginInput);
            }
            else if (input.Password != user.Password) // Nhập sai mật khẩu
            {
                TempData["msg"] = "Bạn nhập sai mật khẩu!";
                return View("./Views/Shared/Login/Login.cshtml", accountLoginInput);
            }
            else // Đúng mật khẩu và chuyển hướng loại tài khoản
            {
                var userSession = new UserLogin();
                userSession.UserName = user.Username;
                userSession.AccountType = user.AccountType;
                HttpContext.Session.SetString("LoginSession", JsonConvert.SerializeObject(userSession));// set Student Session thành 1 JsonConvert 

                if (user.AccountType == "user")
                    return RedirectToAction("Index", "User");
                else if (user.AccountType == "super")
                {
                    if (user.Username == "BanNhanSu") return RedirectToAction("Index", "BNS");
                    else return RedirectToAction("Index", "BNS");
                }   
                    
                else if (user.AccountType == "admin")
                    return RedirectToAction("Index", "Admin");
                else
                    return View("./Views/Shared/Login/Login.cshtml", accountLoginInput);
            }
        }
        // End Index

        // Đổi mật khẩu
        ChangePasswordInput changePasswordInput = new ChangePasswordInput();
        public IActionResult ChangeToChangePassword() //Action đệm, tránh HttpPost
        {
            accountLoginInput = new AccountLoginInput();
            return RedirectToAction("ChangePassword", "Login");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View("./Views/Shared/Login/ChangePassword.cshtml", changePasswordInput);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordInput input)
        {
            changePasswordInput = input;
            var userSession = JsonConvert.DeserializeObject<UserLogin>(HttpContext.Session.GetString("LoginSession"));
            Account user = AccountDAO.Instance.GetAccountbyUsername_Excel(userSession.UserName);
            if (input.OldPassword == null)
            {
                TempData["msg"] = "Vui lòng nhập mật khẩu cũ";
                return View("./Views/Shared/Login/ChangePassword.cshtml", changePasswordInput);
            }
            if (input.OldPassword != user.Password)
            {
                TempData["msg"] = "Bạn nhập sai mật khẩu hiện tại!";
                return View("./Views/Shared/Login/ChangePassword.cshtml", changePasswordInput);
            }

            if (input.NewPassword == null)
            {
                TempData["msg"] = "Vui lòng nhập mật khẩu mới";
                return View("./Views/Shared/Login/ChangePassword.cshtml", changePasswordInput);
            }
            if (input.ReNewPassword == null)
            {
                TempData["msg"] = "Vui lòng xác nhận lại mật khẩu mới";
                return View("./Views/Shared/Login/ChangePassword.cshtml", changePasswordInput);
            }


            if (input.OldPassword == user.Password)
            {
                if (input.NewPassword != input.ReNewPassword) TempData["msg"] = "Mật khẩu xác nhận không trùng khớp!";
                else
                {
                    TempData["msg"] = "Đổi mật khẩu thành công!";

                    AccountDAO.Instance.ChangePassword(user.Username, input.NewPassword);
                    changePasswordInput = new ChangePasswordInput();
                    return RedirectToAction("ChangeToLoginIndex", "Login");
                }
            }
            return View("./Views/Shared/Login/ChangePassword.cshtml", changePasswordInput);

        }

        //
        public IActionResult ChangeToForgotPassword() //Action đệm, tránh HttpPost
        {
            accountLoginInput = new AccountLoginInput();
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
