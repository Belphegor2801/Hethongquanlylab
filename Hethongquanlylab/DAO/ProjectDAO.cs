using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hethongquanlylab.Models;
using System.Data;
using System.Data.SqlClient;
using OfficeOpenXml;
using System.IO;
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

        private ExcelPackage OpenFile() // Mở file
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/project.xlsx"));
            return package;
        }

        private Project LoadData(ExcelWorksheet workSheet, int row)
        {
            int j = 1;
            var id = workSheet.Cells[row, j++].Value;
            var name = workSheet.Cells[row, j++].Value;
            var subid = workSheet.Cells[row, j++].Value;
            var startday = workSheet.Cells[row, j++].Value;
            var endday = workSheet.Cells[row, j++].Value;
            var projecttype = workSheet.Cells[row, j++].Value;
            var status = workSheet.Cells[row, j++].Value;
            var unit = workSheet.Cells[row, j++].Value;

            var ID = id == null ? "N/A" : id.ToString();
            var SubID = subid == null ? "N/A" : subid.ToString();
            var Name = name == null ? "N/A" : name.ToString();
            var Startday = startday == null ? "N/A" : startday.ToString();
            var Endday = endday == null ? "N/A" : endday.ToString();
            var Projecttype = projecttype == null ? "N/A" : projecttype.ToString();
            var Status = status == null ? "N/A" : status.ToString();
            var Unit = unit == null ? "N/A" : unit.ToString();

            var project = new Project(ID, SubID, Name, Startday, Endday, Projecttype, Status, Unit);
            return project;
        }

        private ExcelWorksheet UpdateData(ExcelWorksheet workSheet, Project project, int row) // Update dữ liệu đến hàng row trong workSheet
        {
            int j = 1;
            workSheet.Cells[row, j++].Value = row - 1;
            workSheet.Cells[row, j++].Value = project.Name;
            workSheet.Cells[row, j++].Value = project.SubID;
            workSheet.Cells[row, j++].Value = project.Startday;
            workSheet.Cells[row, j++].Value = project.Endday;
            workSheet.Cells[row, j++].Value = project.ProjectType;
            workSheet.Cells[row, j++].Value = project.Status;
            workSheet.Cells[row, j++].Value = project.Unit;

            return workSheet;
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

        public List<Project> GetProjectList_Excel(string sheetName)
        {
            var package = OpenFile();
            List<Project> projectList = new List<Project>();// mở file excel
            ExcelWorksheet workSheet;
            try
            {
                workSheet = package.Workbook.Worksheets[sheetName];
            }
            catch
            {
                workSheet = package.Workbook.Worksheets.Add(sheetName);
            }

            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                var project = LoadData(workSheet, i);
                projectList.Add(project);
                i++;
            }
            return projectList;
        }

        public Project GetProjectModelbyId_Excel(string sheetName, string projectid)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[sheetName];
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                var id = workSheet.Cells[i, 1].Value;
                string ID = id == null ? "N/A" : id.ToString();
                if (ID == projectid)
                {
                    var project = LoadData(workSheet, i);
                    return project;
                }
            }
            return null;
        }

        public void EditProject(string sheetName, Project project)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[sheetName];
            int i = findRow(workSheet, project.Id);
            workSheet = UpdateData(workSheet, project, i);
            package.Save();

        }

        public void AddProject(string sheetName, Project project) // Thêm mới quy trình vào sheetName
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[sheetName];
            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                i++;
            }

            if (project.SubID == "SubID")
            {
                project.SubID = sheetName + (i - 1).ToString();
            }

            int lastRow = i;
            workSheet = UpdateData(workSheet, project, lastRow);
            package.Save();
        }

        public void DeleteProject(string sheetName, Project project)
        {

        }
        
    }
}
