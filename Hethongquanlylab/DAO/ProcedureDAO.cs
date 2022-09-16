using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hethongquanlylab.Models;
using Hethongquanlylab.Common;
using OfficeOpenXml;

namespace Hethongquanlylab.DAO
{
    public class ProcedureDAO
    {
        private static ProcedureDAO instance;
        public static ProcedureDAO Instance
        {
            get { if (instance == null) instance = new ProcedureDAO(); return ProcedureDAO.instance; }
            private set { ProcedureDAO.instance = value; }
        }

        private ProcedureDAO() { }

        private ExcelPackage OpenFile() // Mở file
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/procedures.xlsx"));
            return package;
        }

        private Procedure LoadData(ExcelWorksheet workSheet, int row) // Load dữ liệu từ hàng row trong workSheet
        {
            int j = 1;
            var id = workSheet.Cells[row, j++].Value;
            var subid = workSheet.Cells[row, j++].Value;
            var name = workSheet.Cells[row, j++].Value;
            var unit = workSheet.Cells[row, j++].Value;
            var senddate = workSheet.Cells[row, j++].Value;
            var content= workSheet.Cells[row, j++].Value;
            var link = workSheet.Cells[row, j++].Value;
            var v1 = workSheet.Cells[row, j++].Value;
            var bdhReply = workSheet.Cells[row, j++].Value;
            var v2 = workSheet.Cells[row, j++].Value;
            var bcvReply = workSheet.Cells[row, j++].Value;
            var v3 = workSheet.Cells[row, j++].Value;
            var nslReply = workSheet.Cells[row, j++].Value;
            var status = workSheet.Cells[row, j++].Value;
            var eventLog = workSheet.Cells[row, j++].Value;

            string ID = id == null ? "N/A" : id.ToString();
            string SubID = subid == null ? "N/A" : subid.ToString();
            string Name = name == null ? "N/A" : name.ToString();
            string Unit = unit == null ? "N/A" : unit.ToString();
            string Senddate = senddate == null ? "N/A" : senddate.ToString();
            string Content = content == null ? "N/A" : content.ToString();
            bool V1;
            try { V1 = v1 == null ? false : Convert.ToBoolean(v1.ToString()); }
            catch { V1 = false; }
            string BDHRePly = bdhReply == null ? "Chưa có phản hồi" : bdhReply.ToString();
            bool V2;
            try { V2 = v2 == null ? false : Convert.ToBoolean(v2.ToString()); }
            catch { V2 = false; }
            string BCVRePly = bcvReply == null ? "Chưa có phản hồi" : bcvReply.ToString();
            bool V3;
            try { V3 = v3 == null ? false : Convert.ToBoolean(v3.ToString()); }
            catch { V3 = false; }
            string NSLReply = nslReply == null ? "Chưa có phản hồi" : nslReply.ToString();
            string Status = status == null ? "Chưa duyệt" : status.ToString();
            string Link = link == null ? "N/A" : link.ToString();

            Procedure procedure = new Procedure(ID, SubID, Name, Unit, Senddate, Content, V1, BDHRePly, V2, BCVRePly, V3, NSLReply, Status, Link);
            return procedure;
        } 

        private ExcelWorksheet UpdateData(ExcelWorksheet workSheet, Procedure procedure, int row) // Update dữ liệu đến hàng row trong workSheet
        {
            int j = 1;
            workSheet.Cells[row, j++].Value = procedure.ID;
            workSheet.Cells[row, j++].Value = procedure.SubID;
            workSheet.Cells[row, j++].Value = procedure.Name;
            workSheet.Cells[row, j++].Value = procedure.Unit;
            workSheet.Cells[row, j++].Value = procedure.Senddate;
            workSheet.Cells[row, j++].Value = procedure.Content;
            workSheet.Cells[row, j++].Value = procedure.Link;
            workSheet.Cells[row, j++].Value = procedure.V1;
            workSheet.Cells[row, j++].Value = procedure.BdhReply;
            workSheet.Cells[row, j++].Value = procedure.V2;
            workSheet.Cells[row, j++].Value = procedure.BcvReply;
            workSheet.Cells[row, j++].Value = procedure.V3;
            workSheet.Cells[row, j++].Value = procedure.NSLReply;
            workSheet.Cells[row, j++].Value = procedure.Status;
            return workSheet;
        }

        private void resetKey(string sheetName) // Cập nhật lại key theo thứ tự số tự nhiên giảm dần
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[sheetName];
            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                i++;
            }

            int rowCount = i; // Sô hàng + 1
            i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                workSheet.Cells[i, 1].Value = rowCount - i;
                i++;
            }
            package.Save();
        }

        private int findRow(ExcelWorksheet workSheet, string key, int var = 0)
        {
            if (var == 0)
            {
                int i = 2;
                while (workSheet.Cells[i, 1].Value != null)
                {
                    var id = workSheet.Cells[i, 1].Value;
                    string ID = id == null ? "N/A" : id.ToString();
                    if (ID == key)
                    {
                        break;
                    }
                    i++;
                }
                return i;
            }
            else
            {
                int i = 2;
                while (workSheet.Cells[i, 1].Value != null)
                {
                    var subid = workSheet.Cells[i, 2].Value;
                    string SubID = subid == null ? "N/A" : subid.ToString();
                    if (SubID == key)
                    {
                        break;
                    }
                    i++;
                }
                return i;
            }
            
        }

        public List<Procedure> GetProcedureList_Excel(string sheetName) // Lấy danh sách quy trình
        {
            var package = OpenFile();
            ExcelWorksheet workSheet;
            try
            {
                workSheet = package.Workbook.Worksheets[sheetName];
            }
            catch
            {
                workSheet = package.Workbook.Worksheets.Add(sheetName);
            }
            
            List<Procedure> procedureList = new List<Procedure>();
            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                Procedure procedure = LoadData(workSheet, i);
                procedureList.Add(procedure);
                i++;
            }
            package.Save();
            return procedureList;
        }

        public Procedure GetProcedureModel_Excel(string sheetName, string procedureid) // Lấy thông tin quy trình có ID = procedureID
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[sheetName];
            int i = findRow(workSheet, procedureid);
            Procedure procedure = LoadData(workSheet, i);
            return procedure;
        }

        public void AddProcedure(string sheetName, Procedure procedure) // Thêm mới quy trình vào sheetName
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[sheetName];
            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                i++;
            }

            if (procedure.SubID == "SubID")
            {
                procedure.SubID = sheetName + (i - 1).ToString();
            }

            int lastRow = i;
            workSheet = UpdateData(workSheet, procedure, lastRow);
            package.Save();
            resetKey(sheetName);
        }

        private string getSubID(string sheetName, string procedureID) // Lấy subID của quy trình
        {
            var package = OpenFile();
            string ProcedureSubID = "";
            ExcelWorksheet workSheet = package.Workbook.Worksheets[sheetName];

            int i = findRow(workSheet, procedureID);
            ProcedureSubID = workSheet.Cells[i, 2].Value.ToString();
            return ProcedureSubID;
        }
         
        private void deleteProcedure(string sheetName, string procedureSubID)  // Xóa ở Sheet [sheetName]
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[sheetName];
            int i = findRow(workSheet, procedureSubID, 1);

            workSheet.DeleteRow(i);
            workSheet.Cells[10, 10].Value = "hang xoa = " + i;
            workSheet.Cells[11, 10].Value = "subid=" + procedureSubID;
            package.Save();
            resetKey(sheetName);
        }

        public void DeleteProcedure(string sheetName, string procedureid)
        {
            string ProcedureSubID = getSubID(sheetName, procedureid);
            deleteProcedure(sheetName, ProcedureSubID);
            deleteProcedure("Ban Điều Hành duyệt", ProcedureSubID);
            deleteProcedure("Ban Cố Vấn duyệt", ProcedureSubID);
            deleteProcedure("Nhà Sáng Lập duyệt", ProcedureSubID);

        } // Xóa ở tất cả các nhánh

        public void EditProcedure(string sheetName, Procedure procedure)
        {
            DeleteProcedure(sheetName, procedure.ID); // Xóa ở tất cả các sheet
            AddProcedure(sheetName, procedure); // Thêm mới vào cuối sheet cần thêm
            resetKey(sheetName);
        }

        public void SendToApproval(string sheetName, Procedure procedure)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[sheetName];
            int i = findRow(workSheet, procedure.SubID, 0);
            workSheet.DeleteRow(i);
            package.Save();
            AddProcedure(sheetName, procedure);
        }      

        public void ReturnProcedure(string unit, Procedure procedure, string feedback) // Trả lại quy trình của đơn vị unit
        {
            procedure.Status = unit + " trả lại";
            procedure.V1 = false;
            procedure.V2 = false;
            procedure.V3 = false;
            if (unit == "Ban Điều Hành")
            {
                procedure.BdhReply = feedback;
            }
            if (unit == "Ban Cố Vấn")
            {
                procedure.BcvReply = feedback;
            }
            EditProcedure(procedure.Unit, procedure); // Đã có xóa ở sheet này
        }

        public void ApprovalProcedure(string unit, Procedure procedure, string feedback)
        {
            procedure.Status = unit + " đã duyệt";
            if (unit == "Ban Điều Hành")
            {
                procedure.V1 = true;
                procedure.BdhReply = feedback;
            }
            if (unit == "Ban Cố Vấn")
            {
                procedure.V2 = true;
                procedure.BcvReply = feedback;
            }
            if (unit == "Nhà Sáng Lập")
            {
                procedure.V3 = true;
                procedure.NSLReply = feedback;
            }
            EditProcedure(procedure.Unit, procedure); // Đã xóa ở sheet này

            AddProcedure(unit + " duyệt", procedure); // Thêm lại vào sheet này
            if (unit == "Ban Cố Vấn")
            {
                AddProcedure("Ban Điều Hành duyệt", procedure); // Thêm lại vào sheet của BĐH
            }
            if (unit == "Nhà Sáng Lập")
            {
                AddProcedure("Ban Điều Hành duyệt", procedure); // Thêm lại vào sheet của BĐH
                AddProcedure("Ban Cố Vấn duyệt", procedure); // Thêm lại vào sheet của BCV
            }
        }

    }
}
