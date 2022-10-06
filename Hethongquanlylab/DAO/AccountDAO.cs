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
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/account.xlsx"));

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
                Account account = new Account((i - 1).ToString(), username, password, accountType);
                accountList.Add(account);
            }
            return accountList;
        }
        
        public Account GetAccountbyUsername_Excel(string accname)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/account.xlsx"));

            // lấy ra sheet đầu tiên để thao tác
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            // duyệt tuần tự từ dòng thứ 2 đến dòng cuối cùng của file. lưu ý file excel bắt đầu từ số 1 không phải số 0
            for (int i = workSheet.Dimension.Start.Row; i <= workSheet.Dimension.End.Row; i++)
            {
                // biến j biểu thị cho một column trong file
                int j = 1;

                // lấy ra cột họ tên tương ứng giá trị tại vị trí [i, 1]. i lần đầu là 2
                // tăng j lên 1 đơn vị sau khi thực hiện xong câu lệnh
                string username = workSheet.Cells[i, j++].Value.ToString();
                if(username == accname)
                {
                    string password = workSheet.Cells[i, j++].Value.ToString();
                    string accountType = workSheet.Cells[i, j++].Value.ToString();
                    Account account = new Account((i - 1).ToString(), username, password, accountType);
                    return account;
                }    
            }
            return null;
        }

        public void DeleteAccount(String id)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/account.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                var key = workSheet.Cells[i, 1].Value;
                string Key = key == null ? "N/A" : key.ToString();
                if (Key == id)
                {
                    break;
                }
                i++;
            }
            workSheet.DeleteRow(i);
            package.Save();
        }

        public void AddAccount(Account account)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/account.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                i++;
            }

            int j = 1;
            workSheet.Cells[i, j++].Value = account.Username;
            workSheet.Cells[i, j++].Value = account.Password;
            workSheet.Cells[i, j++].Value = account.AccountType;

            package.Save();
        }

        public void ChangePassword(string accname, string newPass)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/account.xlsx"));

            // lấy ra sheet đầu tiên để thao tác
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            // duyệt tuần tự từ dòng thứ 2 đến dòng cuối cùng của file. lưu ý file excel bắt đầu từ số 1 không phải số 0
            for (int i = workSheet.Dimension.Start.Row; i <= workSheet.Dimension.End.Row; i++)
            {
                // biến j biểu thị cho một column trong file
                int j = 1;

                // lấy ra cột họ tên tương ứng giá trị tại vị trí [i, 1]. i lần đầu là 2
                // tăng j lên 1 đơn vị sau khi thực hiện xong câu lệnh
                string username = workSheet.Cells[i, j++].Value.ToString();
                if (username == accname)
                {
                    workSheet.Cells[i, j++].Value = newPass;
                    break;
                }
            }
            package.Save();
        }
    }
}
