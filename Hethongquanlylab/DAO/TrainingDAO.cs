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
        
        public List<Training> GetTrainingList_Excel()
        {
            List<Training> trainingList = new List<Training>();// mở file excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/training.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            int i = 2;
            while (workSheet.Cells[i, 1].Value != null)
            {
                int j = 1;
                int id = Convert.ToInt32(workSheet.Cells[i, 1].Value);
                string name = workSheet.Cells[i, 2].Value.ToString();
                string link = workSheet.Cells[i, 3].Value.ToString();
                string sDate = (workSheet.Cells[i, 4].Value).ToString();
                string date;
                try
                {
                    double day = Convert.ToDouble(sDate);
                    DateTimeFormatInfo fmt = (new CultureInfo("fr-FR")).DateTimeFormat;
                    date = DateTime.FromOADate(day).ToString("d", fmt);
                }
                catch
                {
                    date = sDate;
                }
                string unit = workSheet.Cells[i, 5].Value.ToString();
                Training training = new Training(id, name, link, date, unit);
                trainingList.Add(training);
                i++;
            }
            return trainingList;
        }

        public Training GetTrainingModelbyId_Excel(int trainingid)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(new FileInfo("./wwwroot/data/training.xlsx"));
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                int j = 1;
                int id = Convert.ToInt32(workSheet.Cells[i, 1].Value);
                if(id == trainingid)
                {
                    string name = workSheet.Cells[i, 2].Value.ToString();
                    string link = workSheet.Cells[i, 3].Value.ToString();
                    string sDate = (workSheet.Cells[i, 4].Value).ToString();
                    string date;
                    try
                    {
                        double day = Convert.ToDouble(sDate);
                        DateTimeFormatInfo fmt = (new CultureInfo("fr-FR")).DateTimeFormat;
                        date = DateTime.FromOADate(day).ToString("d", fmt);
                    }
                    catch
                    {
                        date = sDate;
                    }
                    string unit = workSheet.Cells[i, 5].Value.ToString();
                    Training training = new Training(id, name, link, date, unit);
                    return training;
                }
            }
            return null;
        }
    }
}
