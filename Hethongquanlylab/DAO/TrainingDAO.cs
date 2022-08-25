using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hethongquanlylab.Models;
using System.Data;
using System.Data.SqlClient;
using OfficeOpenXml;
using System.IO;

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
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                int j = 1;
                int id = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                string name = workSheet.Cells[i, j++].Value.ToString();
                string link = workSheet.Cells[i, j++].Value.ToString();
                Training training = new Training(id, name, link);
                trainingList.Add(training);
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
                int id = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                if(id == trainingid)
                {
                    string name = workSheet.Cells[i, j++].Value.ToString();
                    string link = workSheet.Cells[i, j++].Value.ToString();
                    Training training = new Training(id, name, link);
                    return training;
                }
            }
            return null;
        }
    }
}
