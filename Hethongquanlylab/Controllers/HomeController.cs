using Hethongquanlylab.Models;
using Hethongquanlylab.DAO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
 

namespace Hethongquanlylab.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            String page;
            var urlQuery = Request.HttpContext.Request.Query;
            page = urlQuery["page"];
            page = page == null ? "1" : page;
            int currentPage = Convert.ToInt32(page);
            ItemDisplay<Notification> notificationList = new ItemDisplay<Notification>();
            notificationList.CurrentPage = currentPage;

            List<Notification> notifications = NotificationDAO.Instance.GetNotificationList_Excel();

            notificationList.Paging(notifications, 5);

            if (notificationList.PageCount > 0)
            {
                if (notificationList.CurrentPage > notificationList.PageCount) notificationList.CurrentPage = notificationList.PageCount;
                if (notificationList.CurrentPage < 1) notificationList.CurrentPage = 1;
                if (notificationList.CurrentPage != notificationList.PageCount)
                    try
                    {
                        notificationList.Items = notificationList.Items.GetRange((notificationList.CurrentPage - 1) * notificationList.PageSize, notificationList.PageSize);
                    }
                    catch { }

                else
                    notificationList.Items = notificationList.Items.GetRange((notificationList.CurrentPage - 1) * notificationList.PageSize, notificationList.Items.Count % notificationList.PageSize == 0 ? notificationList.PageSize : notificationList.Items.Count % notificationList.PageSize);
            }
            return View("~/Views/Home/Home.cshtml", notificationList);
        }

        [HttpPost]
        public IActionResult Index(int currentPage = 1)
        {
            return RedirectToAction("Index", new { page = currentPage });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult NotificationDetail()
        {
            var reqUrl = Request.HttpContext.Request;
            var urlPath = reqUrl.Path;
            var CurrentID = urlPath.ToString().Split('/').Last();
            var currenId = Convert.ToInt32(CurrentID);

            var notification = NotificationDAO.Instance.GetNotificationModelbyId_Excel(currenId);
            var item = new ItemDetail<Notification>(notification, "Home");
            return View("./Views/Home/NotificationDetail.cshtml", item);
        }
    }
}
