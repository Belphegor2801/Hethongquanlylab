using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Hethongquanlylab.Models;
using Hethongquanlylab.DAO;
using Hethongquanlylab.Common;
using OfficeOpenXml;
using System.IO;
using System.Data;
using OfficeOpenXml.Table;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Hethongquanlylab.Models.Login;

namespace Hethongquanlylab.Controllers.Super.BanNhanSu
{
    public class BNSController: SuperController
    {
        public BNSController()
        {
            unit = "Ban Nhân Sự";
            unitVar = "BNS";
        }

        // Upload avt: trong Thêm thành viên

        public override IActionResult AddMember()
        {
            var urlQuery = Request.HttpContext.Request.Query;
            String avt = urlQuery["avt"];
            avt = avt == null ? "default.jpg" : avt; // Đặt avt mặc định nếu không up avt lên
            return View(String.Format("./Views/{0}/Members/AddMember.cshtml", unitVar), new List<string>() { unit, avt });
        }


        [HttpPost]
        public override IActionResult UploadAvt(string var, string key, IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
        {
            string fileName = $"{hostingEnvironment.WebRootPath}/img/avt/{file.FileName}";
            // Dẩy file vào thư mục
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }
            TempData["avt"] = file.FileName; // Lưu tên vào TempData => Lưu vào Excel
            if (var == "edit")
                return RedirectToAction("EditMember", new { avt = file.FileName, Key = key });
            else
            {
                return RedirectToAction("AddMember", new { avt = file.FileName });
            }
        }

        [HttpPost]
        public override IActionResult AddMember(String sortOrder, String searchString, String searchField, string IsAdd, string MembersVar, String Key, String LabID, String Name, String Sex, String Birthday, String Gen, String Phone, String Email, String Address, String Specicalization, String University, String Unit, String Position, bool IsLT, bool IsPassPTBT)
        {
            String avt = TempData["avt"] == null ? "default.jpg" : TempData["avt"].ToString();
            var unit = Unit == null ? "Chưa có" : Unit;
            var position = Position == null ? "Chưa có" : Position;
            var phone = Phone == null ? "N/A" : Phone;
            var email = Email == null ? "N/A" : Email;
            var address = Address == null ? "N/A" : Address;
            var specializaion = Specicalization == null ? "N/A" : Specicalization;
            var university = University == null ? "N/A" : University;
            var newMember = new Member(LabID, avt, Name, Sex, Birthday, Gen, phone, email, address, specializaion, university, unit, position, IsLT, IsPassPTBT);
            UserDAO.Instance.AddMember(newMember);
            return RedirectToAction("Member");
        }
        // End: thêm thành viên
    }
}
