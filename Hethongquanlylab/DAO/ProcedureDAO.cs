using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Hethongquanlylab.Models;
using OfficeOpenXml;
using System.IO;
using System.Globalization;

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


        public List<Procedure> GetProcedureList(string unit, string tb = "Process") // Lấy danh sách quy trình
        {
            List<Procedure> items = DataProvider<Procedure>.Instance.GetListItem("Unit", unit, tb);
            items.Reverse();
            return items;
        }

        public Procedure GetProcedureModel(string ID, string tb = "Process") // Lấy thông tin quy trình có ID = procedureID
        {
            Procedure item = DataProvider<Procedure>.Instance.GetItem("ID", ID, tb);
            return item;
        }

        public void AddProcedure(Procedure procedure, string tableName = "Process") // Thêm mới quy trình vào sheetName
        {
            DataTable data = DataProvider<Procedure>.Instance.LoadData(tableName);
            DataRow newProcedure = data.NewRow();

            var allAttr = typeof(Procedure).GetProperties(); // Lấy danh sách attributes của class Procedure

            foreach (var attr in allAttr)
                newProcedure[attr.Name] = attr.GetValue(procedure);

            data.Rows.Add(newProcedure);
            DataProvider<Procedure>.Instance.UpdateData(data, tableName);
        }

        private void deleteProcedure(string col, string Key, string tableName = "Process")  // Xóa ở Sheet [sheetName]
        {
            DataProvider<Procedure>.Instance.DeleteItem(col, Key, tableName);
        }

        public void DeleteProcedure(string ID)
        {
            deleteProcedure("ID", ID);
            deleteProcedure("SubID", ID, "ProcedureApproval");

        } // Xóa ở tất cả các nhánh

        public void EditProcedure(Procedure procedure, string tableName = "Process")
        {
            DeleteProcedure(procedure.ID); // Xóa ở tất cả các sheet
            AddProcedure(procedure, tableName); // Thêm mới vào cuối sheet cần thêm
        }

        public void SendToApproval(Procedure procedure, string unit)
        {
            String SubID = (Convert.ToInt32(procedure.ID) + 1).ToString() ; // Do edit = delete + add (Primary key ID sẽ tăng 1)
            deleteProcedure("SubID", procedure.ID, "ProcedureApproval");
            procedure.SubID = SubID;
            if (unit == "Ban Điều Hành")
            {
                procedure.Unit = "Ban Cố Vấn";
                AddProcedure(procedure, "ProcedureApproval"); 
                procedure.Unit = "Nhà Sáng Lập";
                AddProcedure(procedure, "ProcedureApproval");
            }
            else if (unit == "Ban Cố Vấn")
            {
                procedure.Unit = "Nhà Sáng Lập";
                AddProcedure(procedure, "ProcedureApproval");
            }
            else
            {
                procedure.Unit = "Ban Điều Hành";
                AddProcedure(procedure, "ProcedureApproval");
                procedure.Unit = "Ban Cố Vấn";
                AddProcedure(procedure, "ProcedureApproval");
                procedure.Unit = "Nhà Sáng Lập";
                AddProcedure(procedure, "ProcedureApproval");
            }
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
            EditProcedure(procedure); // Đã có xóa ở sheet này
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

            EditProcedure(procedure);
            AddProcedure(procedure); // Thêm lại vào sheet này
            if (unit == "Ban Cố Vấn")
            {
                procedure.Unit = "Ban Điều Hành";
                AddProcedure(procedure, "ProcedureApproval");
            }
            if (unit == "Nhà Sáng Lập")
            {
                procedure.Unit = "Ban Điều Hành";
                AddProcedure(procedure, "ProcedureApproval");
                procedure.Unit = "Ban Cố Vấn";
                AddProcedure(procedure, "ProcedureApproval");
            }
        }

    }
}
