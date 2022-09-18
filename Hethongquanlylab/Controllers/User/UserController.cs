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
            return View("./Views/User/Infor/EditInfor.cshtml");
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
