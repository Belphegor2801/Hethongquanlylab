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
    public class TrainingDAO
    {
        private static TrainingDAO instance;
        public static TrainingDAO Instance
        {
            get { if (instance == null) instance = new TrainingDAO(); return TrainingDAO.instance; }
            private set { TrainingDAO.instance = value; }
        }

        private TrainingDAO() { }

        private ExcelPackage OpenFile() // Mở file
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/trainings.xlsx"));
            return package;
        }

        private Training LoadData(ExcelWorksheet workSheet, int row)
        {
            int j = 1;
            var id = workSheet.Cells[row, j++].Value;
            var subid = workSheet.Cells[row, j++].Value;
            var name = workSheet.Cells[row, j++].Value;
            var link = workSheet.Cells[row, j++].Value;
            var date = workSheet.Cells[row, j++].Value;
            var unit = workSheet.Cells[row, j++].Value;
            var content = workSheet.Cells[row, j++].Value;

            var ID = id == null ? "N/A" : id.ToString();
            var SubID = subid == null ? "N/A" : subid.ToString();
            var Name = name == null ? "N/A" : name.ToString();
            var Link = link == null ? "N/A" : link.ToString();
            var Date = date == null ? "N/A" : date.ToString();
            var Unit = unit == null ? "N/A" : unit.ToString();
            var Content = content == null ? "N/A" : content.ToString();

            var training = new Training(ID, SubID, Name, Link, Date, Unit, Content);
            return training;
        }

        private ExcelWorksheet UpdateData(ExcelWorksheet workSheet, Training training, int row) // Update dữ liệu đến hàng row trong workSheet
        {
            int j = 1;
            workSheet.Cells[row, j++].Value = row - 1;
            workSheet.Cells[row, j++].Value = training.SubID;
            workSheet.Cells[row, j++].Value = training.Name;
            workSheet.Cells[row, j++].Value = training.Link;
            workSheet.Cells[row, j++].Value = training.Date;
            workSheet.Cells[row, j++].Value = training.Unit;
            workSheet.Cells[row, j++].Value = training.Content;

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

        public List<Training> GetTrainingList_Excel(string unit)
        {
            var package = OpenFile();
            List<Training> trainingList = new List<Training>();// mở file excel
            ExcelWorksheet workSheet;
            workSheet = package.Workbook.Worksheets.First();

            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                var u = workSheet.Cells[i, 6].Value;
                string U = u == null ? "N/A" : u.ToString();
                if (U == unit)
                {
                    var training = LoadData(workSheet, i);
                    trainingList.Add(training);
                }
                i++;
            }
            return trainingList;
        }

        public Training GetTrainingModelbyId_Excel(string sheetName, string trainingid)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[sheetName];
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                var id = workSheet.Cells[i, 1].Value;
                string ID = id == null ? "N/A" : id.ToString();
                if(ID == trainingid)
                {
                    var training = LoadData(workSheet, i);
                    return training;
                }
            }
            return null;
        }

        public void EditTraing(string sheetName, Training training)
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[sheetName];
            int i = findRow(workSheet, training.ID);
            workSheet = UpdateData(workSheet, training, i);
            package.Save();

        }

        public void AddTraining(string sheetName, Training training) // Thêm mới quy trình vào sheetName
        {
            var package = OpenFile();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[sheetName];
            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                i++;
            }

            if (training.SubID == "SubID")
            {
                training.SubID = sheetName + (i - 1).ToString();
            }

            int lastRow = i;
            workSheet = UpdateData(workSheet, training, lastRow);
            package.Save();
        }

        public void DeleteTraining(string sheetName, Training training)
        {

        }
    }
}
