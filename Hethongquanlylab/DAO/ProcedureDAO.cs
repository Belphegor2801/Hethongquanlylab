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
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/training.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                int j = 1;
                int id = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                string name = workSheet.Cells[i, j++].Value.ToString();
                string link = workSheet.Cells[i, j++].Value.ToString();
                string unit = workSheet.Cells[i, j++].Value.ToString();
                string status = workSheet.Cells[i, j++].Value.ToString();
                Procedure procedure = new Procedure(id, name, link, unit,status);
                procedureList.Add(procedure);
                i++;
            }
            return procedureList;
        }

        public Procedure GetProcedureModel_Excel(int procedureid)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/training.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                int j = 1;
                int id = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                if (id == procedureid)
                {
                    string name = workSheet.Cells[i, j++].Value.ToString();
                    string link = workSheet.Cells[i, j++].Value.ToString();
                    string unit = workSheet.Cells[i, j++].Value.ToString();
                    string status = workSheet.Cells[i, j++].Value.ToString();
                    Procedure procedure = new Procedure(id, name, link, unit, status);
                    return procedure;
                }
            }
            return null;
        }

    }
}
