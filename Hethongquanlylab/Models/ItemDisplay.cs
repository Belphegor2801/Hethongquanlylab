using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hethongquanlylab.Models
{
    public class ItemDisplay <T>
    {
        private int pageCount;
        private int pageSize;
        private int currentPage;
        private List<T> items;
        private int itemCount;

        private String sortOrder;
        private String currentSearchString;
        private String currentSearchField;
        private List<String> searchFieldList;

        public int PageCount { get => pageCount; set => pageCount = value; }
        public int PageSize { get => pageSize; set => pageSize = value; }
        public int CurrentPage { get => currentPage; set => currentPage = value; }
        public List<T> Items { get => items; set => items = value; }
        public int ItemCount { get => itemCount; set => itemCount = value; }

        public String SortOrder { get => sortOrder; set => sortOrder = value; }
        public String CurrentSearchString { get => currentSearchString; set => currentSearchString = value; }
        public String CurrentSearchField { get => currentSearchField; set => currentSearchField = value; }
        public List<String> SearchFieldList { get => searchFieldList; set => searchFieldList = value; }

        public Dictionary<string, string> NameVar { get; set; }

        public static Boolean IsAddMember = false;

        public ItemDisplay()
        {
            this.pageSize = 10;
            this.currentPage = 1;
            this.searchFieldList = new List<String>();
            this.NameVar = new Dictionary<string, string>();
            foreach (var attr in typeof(T).GetProperties().ToList())
            {
                this.searchFieldList.Add(attr.Name.ToString());
                this.NameVar.Add(attr.Name.ToString(), attr.Name.ToString());
            }

            this.NameVar["LabID"] = "LabID";
            this.NameVar["Name"] = "Tên";
            this.NameVar["Sex"] = "Giới tính";
            this.NameVar["Birthday"] = "Ngày sinh";
            this.NameVar["Gen"] = "Thế hệ";
            this.NameVar["Unit"] = "Đơn vị";
            this.NameVar["Position"] = "Chức vụ";
        }

        public void Paging(List<T> members, int pageSize)
        {
            this.items = members;
            this.itemCount = members.Count;
            this.pageSize = pageSize;
            
            if ((double)((decimal)this.items.Count() % Convert.ToDecimal(this.pageSize)) == 0)
            {
                this.pageCount = this.items.Count() / this.pageSize;
            }
            else
            {
                double page_Count = (double)((decimal)this.items.Count() / Convert.ToDecimal(this.pageSize));
                this.pageCount = (int)Math.Ceiling(page_Count);
            }

        }
    }
}
