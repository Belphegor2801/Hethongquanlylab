using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hethongquanlylab.Models;
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

        public List<Procedure> GetProcedureList_Excel()
        {
            List<Procedure> procedureList = new List<Procedure>();// mở file excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/procedure.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                int j = 1;
                int id = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                string name = workSheet.Cells[i, j++].Value.ToString();
                string unit = workSheet.Cells[i, j++].Value.ToString();
                var SendDate = workSheet.Cells[i, j++].Value;
                string senddate = SendDate == null? "01/01/1111": SendDate.ToString();
                string content = workSheet.Cells[i, j++].Value.ToString();
                var V1 = workSheet.Cells[i, j++].Value;
                string v1 = V1 == null ? "false" : V1.ToString();
                var V2 = workSheet.Cells[i, j++].Value;
                string v2 = V2 == null ? "false" : V2.ToString();
                var V3 = workSheet.Cells[i, j++].Value;
                string v3 = V3 == null ? "false" : V3.ToString();
                string status = workSheet.Cells[i, j++].Value.ToString();
                string link = workSheet.Cells[i, j++].Value.ToString();
                Procedure procedure = new Procedure(id, name, unit, senddate, content, v1, v2, v3, link);
                procedureList.Add(procedure);
                i++;
            }
            return procedureList;
        }

        public Procedure GetProcedureModel_Excel(int procedureid)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/procedure.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                int id = Convert.ToInt32(workSheet.Cells[i, 1].Value);
                if (id == procedureid)
                {
                    int j = 2;
                    string name = workSheet.Cells[i, j++].Value.ToString();
                    string unit = workSheet.Cells[i, j++].Value.ToString();
                    var SendDate = workSheet.Cells[i, j++].Value;
                    string senddate = SendDate == null ? "01/01/1111" : SendDate.ToString();
                    string content = workSheet.Cells[i, j++].Value.ToString();
                    var V1 = workSheet.Cells[i, j++].Value;
                    string v1 = V1 == null ? "false" : V1.ToString();
                    var V2 = workSheet.Cells[i, j++].Value;
                    string v2 = V2 == null ? "false" : V2.ToString();
                    var V3 = workSheet.Cells[i, j++].Value;
                    string v3 = V3 == null ? "false" : V3.ToString();
                    string status = workSheet.Cells[i, j++].Value.ToString();
                    string link = workSheet.Cells[i, j++].Value.ToString();
                    Procedure procedure = new Procedure(id, name, unit, senddate, content, v1, v2, v3, link);
                    return procedure;
                }
            }
            return null;
        }

        public int GetMaxID()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/procedure.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            return workSheet.Dimension.End.Row;
        }

        public void AddProcedure(Procedure procedure)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/procedure.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i = 3;
            while (workSheet.Cells[i, 1].Value != null)
            {
                i++;
            }

            int lastRow = i;
            workSheet.Cells[lastRow, 1].Value = procedure.ID;
            workSheet.Cells[lastRow, 2].Value = procedure.Name;
            workSheet.Cells[lastRow, 3].Value = procedure.Unit;
            workSheet.Cells[lastRow, 4].Value = procedure.Senddate;
            workSheet.Cells[lastRow, 5].Value = procedure.Content;
            workSheet.Cells[lastRow, 6].Value = procedure.V1;
            workSheet.Cells[lastRow, 7].Value = procedure.V2;
            workSheet.Cells[lastRow, 8].Value = procedure.V3;
            workSheet.Cells[lastRow, 9].Value = procedure.Status;
            workSheet.Cells[lastRow, 10].Value = procedure.Link;
            package.Save();
        }

        public void DeleteProcedure(String id)
        {
            List<Notification> notificationList = new List<Notification>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/procedure.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                string Id = workSheet.Cells[i, 1].Value.ToString();
                if (id == Id)
                {
                    break;
                }
                i++;
            }
            workSheet.DeleteRow(i);
            package.Save();
        }

        public void EditProcedure(Procedure procedure)
        {
            List<Notification> notificationList = new List<Notification>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/procedure.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i;
            for (i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                string Id = workSheet.Cells[i, 1].Value.ToString();
                if (procedure.ID.ToString() == Id)
                {
                    break;
                }
            }
            workSheet.Cells[i, 1].Value = procedure.ID;
            workSheet.Cells[i, 2].Value = procedure.Name;
            workSheet.Cells[i, 3].Value = procedure.Unit;
            workSheet.Cells[i, 4].Value = procedure.Senddate;
            workSheet.Cells[i, 5].Value = procedure.Content;
            workSheet.Cells[i, 6].Value = procedure.V1;
            workSheet.Cells[i, 7].Value = procedure.V2;
            workSheet.Cells[i, 8].Value = procedure.V3;
            workSheet.Cells[i, 9].Value = procedure.Status;
            workSheet.Cells[i, 10].Value = procedure.Link;
            package.Save();
        }

    }
}
