using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Hethongquanlylab.Models;
using System.IO;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using Baseline.ImTools;
using OfficeOpenXml.Style;

namespace Hethongquanlylab.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance;
        public static AccountDAO Instance
        {
            get { if (instance == null) instance = new AccountDAO(); return AccountDAO.instance; }
            private set { AccountDAO.instance = value; }
        }

        private AccountDAO() { }
        public List<Account> GetAccountList_Excel()
        {
            List<Account> accountList = new List<Account>();// mở file excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("data.csv"));

            // lấy ra sheet đầu tiên để thao tác
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            // duyệt tuần tự từ dòng thứ 2 đến dòng cuối cùng của file. lưu ý file excel bắt đầu từ số 1 không phải số 0
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                // biến j biểu thị cho một column trong file
                int j = 1;

                // lấy ra cột họ tên tương ứng giá trị tại vị trí [i, 1]. i lần đầu là 2
                // tăng j lên 1 đơn vị sau khi thực hiện xong câu lệnh
                string username = workSheet.Cells[i, j++].Value.ToString();
                string password = workSheet.Cells[i, j++].Value.ToString();
                string accountType = workSheet.Cells[i, j++].Value.ToString();
                Account account = new Account(username, password, accountType);
                accountList.Add(account);
            }
            return accountList;
        }
        
        public Account GetAccountbyUsername_Excel(string name)
        { 
            var package = new ExcelPackage(new FileInfo("data.csv"));

            // lấy ra sheet đầu tiên để thao tác
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
            // duyệt tuần tự từ dòng thứ 2 đến dòng cuối cùng của file. lưu ý file excel bắt đầu từ số 1 không phải số 0
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                // biến j biểu thị cho một column trong file
                int j = 1;

                // lấy ra cột họ tên tương ứng giá trị tại vị trí [i, 1]. i lần đầu là 2
                // tăng j lên 1 đơn vị sau khi thực hiện xong câu lệnh
                string username = workSheet.Cells[i, j++].Value.ToString();
                if(String.Compare(username,name,false) == 1)
                {
                    string password = workSheet.Cells[i, j++].Value.ToString();
                    string accountType = workSheet.Cells[i, j++].Value.ToString();
                    Account account = new Account(username, password, accountType);
                    return account;
                }    
            }
            return null;
        }
        
    }
}
