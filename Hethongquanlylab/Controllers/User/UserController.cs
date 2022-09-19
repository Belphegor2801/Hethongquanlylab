﻿using Microsoft.AspNetCore.Mvc;
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
using Hethongquanlylab.Common;
using SelectPdf;
using System.Web;

namespace Hethongquanlylab.Controllers.User
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            String page;
            var urlQuery = Request.HttpContext.Request.Query;
            page = urlQuery["page"];

            var notificationList = Function.Instance.getNotifications(page);
            string formLink = LinkDAO.Instance.GetLink("Biểu mẫu");
            notificationList.Message = formLink;
            return View("./Views/User/UserHome.cshtml", notificationList);
        }

        [HttpPost]
        public IActionResult Index(int currentPage = 1)
        {
            return RedirectToAction("Index", new {page = currentPage});
        }

        public IActionResult Infor()
        {
            var userSession = JsonConvert.DeserializeObject<UserLogin>(HttpContext.Session.GetString("LoginSession"));
            var user = UserDAO.Instance.GetUserByID_Excel("1");
            return View("./Views/User/Infor/Infor.cshtml", user);
        }

        [HttpPost]
        public IActionResult ExportToPDF(string GridHtml)
        {
            GlobalProperties.HtmlEngineFullPath = Path.GetFullPath("~/bin/Debug/netcoreapp3.1/Select.Html.dep").Replace("~\\", "");
            GridHtml = GridHtml.Replace("StrTag", "<").Replace("EndTag", ">");
            HtmlToPdf pHtml = new HtmlToPdf();
            string baseUrl = string.Format("{0}://{1}",
                       HttpContext.Request.Scheme, HttpContext.Request.Host);
            PdfDocument pdfDocument = pHtml.ConvertHtmlString(GridHtml, baseUrl);
            byte[] pdf = pdfDocument.Save();
            pdfDocument.Close();
            return File(pdf, "application/pdf", "CV.pdf");

        }

        public IActionResult EditInfor()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String CurrentID = urlQuery["Key"]; // Url: .../DeteleMeber?Key={key}
            String avt = urlQuery["avt"];

            var member = UserDAO.Instance.GetUserByID_Excel(CurrentID);
            if (avt != null) member.Avt = avt;
            return View("./Views/User/Infor/EditInfor.cshtml", member);
        }

        [HttpPost]
        public virtual IActionResult UploadAvt(string var, string key, IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
        {
            string fileName = $"{hostingEnvironment.WebRootPath}/img/avt/{file.FileName}";
            // Dẩy file vào thư mục
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }
            TempData["avt"] = file.FileName; // Lưu tên vào TempData => Lưu vào Excel
            return RedirectToAction("EditInfor", new { avt = file.FileName, Key = key });
        }


        [HttpPost]
        public IActionResult EditInfor(String Key, String LabID, String Name, String Sex, String Birthday, String Gen, String Phone, String Email, String Address, String Specicalization, String University, String Unit, String Position, bool IsLT, bool IsPassPTBT)
        {
            String avt = TempData["avt"] == null ? "default.jpg" : TempData["avt"].ToString();
            var member = UserDAO.Instance.GetUserByID_Excel(Key);
            member.Avt = avt;
            member.Name = Name;
            member.Sex = Sex;
            member.Birthday = Birthday;
            member.Phone = Phone;
            member.Email = Email;
            member.Address = Address;
            member.Specialization = Specicalization;
            member.University = University;
            UserDAO.Instance.EditMember(member);
            return RedirectToAction("Infor");
        }

        public IActionResult Training()
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();

            try
            {
                ViewData["currentTraining"] = Convert.ToInt32(CurrentID) - 1;
            }
            catch
            {
                ViewData["currentTraining"] = 0;
            }
            
            var training = TrainingDAO.Instance.GetTrainingList_Excel("All");
            return View("./Views/User/Training.cshtml", training);
        }

        public IActionResult NotificationDetail()
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();
            var currenId = Convert.ToInt32(CurrentID);

            var notification = NotificationDAO.Instance.GetNotificationModelbyId_Excel(currenId);
            return View("./Views/Shared/NotificationDetail.cshtml", notification);
        }
    }
}
