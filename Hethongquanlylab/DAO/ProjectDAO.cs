using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hethongquanlylab.Models;
using System.Globalization;

namespace Hethongquanlylab.DAO
{
    public class ProjectDAO
    {
        private static ProjectDAO instance;
        public static ProjectDAO Instance
        {
            get { if (instance == null) instance = new ProjectDAO(); return ProjectDAO.instance; }
            private set { ProjectDAO.instance = value; }
        }

        private ProjectDAO() { }

        public List<Project> GetProjectList_Excel()
        {
            List<Project> projectList = new List<Project>();// mở file excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/project.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                int j = 1;
                string id = workSheet.Cells[i, 1].Value.ToString();
                string name = workSheet.Cells[i, 2].Value.ToString();
                string sDate = (workSheet.Cells[i, 4].Value).ToString();
                string eDate = workSheet.Cells[i, 5].Value.ToString();
                string StartDay;
                string EndDay;
                try
                {
                    double sdate = Convert.ToDouble(sDate);
                    double edate = Convert.ToDouble(eDate);
                    DateTimeFormatInfo fmt = (new CultureInfo("fr-FR")).DateTimeFormat;
                    StartDay = DateTime.FromOADate(sdate).ToString("d", fmt);
                    EndDay = DateTime.FromOADate(edate).ToString("d", fmt);
                }
                catch
                {
                    StartDay = sDate;
                    EndDay = eDate;
                }
                string projectType = workSheet.Cells[i, 6].Value.ToString();
                string status = workSheet.Cells[i, 7].Value.ToString();
                string unit = workSheet.Cells[i, 8].Value.ToString();
                Project project = new Project(id, name, StartDay, EndDay, projectType, status, unit);
                projectList.Add(project);
                i++;
            }
            return projectList;
        }

        public Project GetProjectModelbyId_Excel(string idProject)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/project.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                int j = 1;
                string id = workSheet.Cells[i, 1].Value.ToString();
                if(idProject == id)
                {
                    string name = workSheet.Cells[i, 2].Value.ToString();
                    string sDate = (workSheet.Cells[i, 4].Value).ToString();
                    string eDate = workSheet.Cells[i, 5].Value.ToString();
                    string StartDay;
                    string EndDay;
                    try
                    {
                        double sdate = Convert.ToDouble(sDate);
                        double edate = Convert.ToDouble(eDate);
                        DateTimeFormatInfo fmt = (new CultureInfo("fr-FR")).DateTimeFormat;
                        StartDay = DateTime.FromOADate(sdate).ToString("d", fmt);
                        EndDay = DateTime.FromOADate(edate).ToString("d", fmt);
                    }
                    catch
                    {
                        StartDay = sDate;
                        EndDay = eDate;
                    }
                    string projectType = workSheet.Cells[i, 6].Value.ToString();
                    string status = workSheet.Cells[i, 7].Value.ToString();
                    string unit = workSheet.Cells[i, 8].Value.ToString();
                    Project project = new Project(id, name, StartDay, EndDay, projectType, status, unit);
                    return project;
                }
                i++;
            }
            return null;
        }
        public void DeleteProject(String id)
        {
            List<Project> projectList = new List<Project>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/project.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i = 3;
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
        public void AddProject(Project project)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/project.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int i = 3;
            while (workSheet.Cells[i, 1].Value != null)
            {
                i++;
            }

            int lastRow = i;
            workSheet.Cells[lastRow, 1].Value = project.Id;
            workSheet.Cells[lastRow, 2].Value = project.Name;
            workSheet.Cells[lastRow, 8].Value = project.Unit;
            workSheet.Cells[lastRow, 4].Value = project.Startday;
            workSheet.Cells[lastRow, 5].Value = project.Endday;
            workSheet.Cells[lastRow, 7].Value = project.ProjectType;
            workSheet.Cells[lastRow, 9].Value = project.Status;
            package.Save();
        }
        public string GetMaxID()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/project.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            return workSheet.Dimension.End.Row.ToString();
        }
    }
}
