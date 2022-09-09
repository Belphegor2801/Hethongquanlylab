using System;
using System.Collections.Generic;
using System.Linq;
using Hethongquanlylab.Models;
using Hethongquanlylab.DAO;
using OfficeOpenXml;
using System.IO;
using System.Data;


namespace Hethongquanlylab.Common
{
    public class Function
    {
        private static Function instance;
        public static Function Instance
        {
            get { if (instance == null) instance = new Function(); return Function.instance; }
            private set { Function.instance = value; }
        }

        public List<T> sortItems<T>(List<T> items, String sortOrder)
        {
            var attrs = typeof(T).GetProperties();
            var result = items.OrderBy(s => attrs.First().GetValue(s, null));
            foreach (var attr in attrs)
            {
                if (sortOrder == attr.Name.ToString())
                {
                    if (attr.Name.ToString() == "Name") result = items.OrderBy(s => attr.GetValue(s, null).ToString().Split(" ").Last());
                    else if (attr.Name.ToString() == "Birthday") result = items.OrderBy(s => attr.GetValue(s, null).ToString().Split("/").Last());
                    else if (attr.Name.ToString().Contains("ID")) result = items.OrderBy(s => Convert.ToInt32(attr.GetValue(s, null)));
                    else
                        result = items.OrderBy(s => attr.GetValue(s, null));
                }
                else if (sortOrder == attr.Name.ToString() + "_desc")
                {
                    if (attr.Name.ToString() == "Name") result = items.OrderByDescending(s => attr.GetValue(s, null).ToString().Split(" ").Last());
                    else if (attr.Name.ToString() == "Birthday") result = items.OrderByDescending(s => attr.GetValue(s, null).ToString().Split("/").Last());
                    else if (attr.Name.ToString().Contains("ID")) result = items.OrderByDescending(s => Convert.ToInt32(attr.GetValue(s, null)));
                    else
                        result = items.OrderByDescending(s => attr.GetValue(s, null));
                }
            }

            return result.ToList();
        }

        public List<T> searchItems<T>(List<T> items, ItemDisplay<T> itemDisplay)
        {
            if (!String.IsNullOrEmpty(itemDisplay.CurrentSearchField))
            {
                if (!String.IsNullOrEmpty(itemDisplay.CurrentSearchString))
                {
                    var attrs = typeof(T).GetProperties();
                    foreach (var attr in attrs)
                    {
                        if (itemDisplay.CurrentSearchField == attr.Name.ToString())
                        {
                            items = items.Where(s => attr.GetValue(s, null).ToString().Contains(itemDisplay.CurrentSearchString)).ToList();
                        }
                    }
                }
            }
            return items;
        }

        public MemoryStream ExportToExcel<T>(List<T> items)
        {
            var memoryStream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(memoryStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("Danh sách thành viên");
                var currentRow = 1;
                // trỏ đến dòng 1 và cột 1 thay giá trị bằng LabID các dòng dưới cx tương tự

                var allAttr = typeof(T).GetProperties(); // Lấy danh sách attributes của class Member
                int col = 1;


                foreach (var attr in allAttr)
                    if (attr.Name != "Avt")
                        worksheet.Cells[currentRow, col++].Value = attr.Name;

                // Lấy tất cả dữ liệu trong database theo thứ tự tăng dần labID
                
                foreach (var item in items)
                {
                    // Dòng thứ 2 trở đi sẽ đổ dữ liệu từ database vào
                    currentRow += 1;
                    col = 1;
                    foreach (var attr in allAttr)
                    {
                        if (attr.Name != "Avt")
                        {
                            object value = attr.GetValue(item);
                            worksheet.Cells[currentRow, col++].Value = value.ToString();
                        }
                    }

                }
                // Trả về dữ liệu dạng xlsx
                using (var stream = new MemoryStream())
                {
                    excelPackage.SaveAs(stream);
                    return stream;
                }
            }
        }
    }
}
