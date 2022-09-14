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

        private ExcelPackage OpenFile()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/procedures.xlsx"));
            return package;
        }

        private Procedure LoadData(ExcelWorksheet workSheet, int row)
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

        private ExcelWorksheet UpdateData(ExcelWorksheet workSheet, Procedure procedure, int row)
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

        private void resetKey()
        {
            var package = OpenFile();

            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                i++;
            }

            int rowCount = i;
            i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                workSheet.Cells[i, 1].Value = rowCount - i;
                i++;
            }
            package.Save();
        }

        private void resetKey(string unit)
        {
            var package = OpenFile();

            ExcelWorksheet workSheet = package.Workbook.Worksheets[unit];

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

        private List<Procedure> getProcedureList(ExcelWorksheet workSheet)
        {
            List<Procedure> procedureList = new List<Procedure>();
            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                Procedure procedure = LoadData(workSheet, i);
                procedureList.Add(procedure);
                i++;
            }
            return procedureList;
        }

        public List<Procedure> GetProcedureList_Excel()
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            return getProcedureList(workSheet);
        }

        public List<Procedure> GetProcedureList_Excel(string unit)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[unit];
            return getProcedureList(workSheet);
        }


        private Procedure getProcedure(ExcelWorksheet workSheet, string procedureid)
        {
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                var id = workSheet.Cells[i, 1].Value;
                string ID = id == null ? "N/A" : id.ToString();
                if (ID == procedureid)
                {
                    Procedure procedure = LoadData(workSheet, i);
                    return procedure;
                }
            }
            return null;
        }

        public Procedure GetProcedureModel_Excel(string procedureid)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            return getProcedure(workSheet, procedureid);
        }

        public Procedure GetProcedureModel_Excel(string unit, string procedureid)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[unit];
            return getProcedure(workSheet, procedureid);
        }

        private ExcelWorksheet addProcedure(ExcelWorksheet workSheet, Procedure procedure)
        {
            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                i++;
            }

            procedure.ID = (i - 1).ToString();
            if (procedure.SubID == "BNS")
            {
                procedure.SubID = "BNS" + (i - 1).ToString();
            }

            int lastRow = i;
            workSheet = UpdateData(workSheet, procedure, lastRow);
            return workSheet;
        }

        public void AddProcedure(Procedure procedure)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            workSheet = addProcedure(workSheet, procedure);
            package.Save();
            resetKey();
        }

        public void AddProcedure(string unit, Procedure procedure)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[unit];
            workSheet = addProcedure(workSheet, procedure);
            package.Save();
            resetKey(unit);
        }

        public void DeleteProcedure(string unit, string procedureid)
        {
            var package = OpenFile();
            string ProcedureSubID = "";
            ExcelWorksheet workSheet = package.Workbook.Worksheets[unit];
            int i = 0;
            
            for (i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                var id = workSheet.Cells[i, 1].Value;
                string ID = id == null ? "N/A" : id.ToString();
                if (ID == procedureid)
                {
                    ProcedureSubID = workSheet.Cells[i, 2].Value.ToString();
                    break;
                }
            }
            workSheet.DeleteRow(i);

            workSheet = package.Workbook.Worksheets.First();

            for (i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                var subid = workSheet.Cells[i, 2].Value;
                string SubID = subid == null ? "N/A" : subid.ToString();
                if (ProcedureSubID == SubID)
                {
                    break;
                }
            }
            workSheet.DeleteRow(i);
            package.Save();
            resetKey();
            resetKey(unit);
        }

        public void EditProcedure(string unit, Procedure procedure)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[unit];

            int i;
            for (i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                var id = workSheet.Cells[i, 1].Value;
                string ID = id == null ? "N/A" : id.ToString();
                if (procedure.ID.ToString() == ID)
                {
                    break;
                }
            }

            int row = i;
            workSheet.DeleteRow(row);
            workSheet = addProcedure(workSheet, procedure);
            package.Save();
            resetKey(unit);
        }

        public void SendToApproval(Procedure procedure)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i;
            for (i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                var subid = workSheet.Cells[i, 2].Value;
                string SubID = subid == null ? "N/A" : subid.ToString();
                if ((procedure.SubID.ToString() == SubID))
                {
                    break;
                }
            }
            workSheet.DeleteRow(i);

            workSheet = addProcedure(workSheet, procedure);
            package.Save();
            resetKey();
        }

        public void BDHFeedbackProcedure(Procedure procedure, string feedback)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i;
            for (i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                string SubID = workSheet.Cells[i, 2].Value.ToString();
                if (procedure.SubID.ToString() == SubID)
                {
                    break;
                }
            }
            procedure.Status = "Ban Điều hành phản hồi";
            workSheet.Cells[i, 14].Value = procedure.Status;
            procedure.BdhReply = feedback;
            workSheet.Cells[i, 9].Value = procedure.BdhReply;
            package.Save();
            UpdateDatatoUnitSheet(procedure.Unit, procedure);
        }
        public void UpdateDatatoUnitSheet(string unit, Procedure procedure)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[unit];

            int i;
            for (i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                string SubID = workSheet.Cells[i, 2].Value.ToString();
                if (procedure.SubID.ToString() == SubID)
                {
                    break;
                }
            }
            EditProcedure(unit, procedure);
        }
        public void BDHApproval(Procedure procedure, string feedback)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i;
            for (i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                string SubID = workSheet.Cells[i, 2].Value.ToString();
                if (procedure.SubID.ToString() == SubID)
                {
                    break;
                }
            }
            workSheet.Cells[i, 8].Value = true;
            procedure.Status = "Ban Điều hành đã duyệt";
            workSheet.Cells[i, 14].Value = procedure.Status;
            procedure.BdhReply = feedback;
            workSheet.Cells[i, 9].Value = procedure.BdhReply;
            package.Save();
            UpdateDatatoUnitSheet(procedure.Unit, procedure);
        }

    }
}
