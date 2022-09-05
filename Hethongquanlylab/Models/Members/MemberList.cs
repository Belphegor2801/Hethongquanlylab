﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hethongquanlylab.Models
{
    public class MemberList
    {
        private int pageCount;
        private int pageSize;
        private int currentPage;
        private List<Member> memberList;
        private int memberCount;

        private String sortOrder;
        private String currentSearchString;
        private String currentSearchField;
        private List<String> searchFieldList;

        public int PageCount { get => pageCount; set => pageCount = value; }
        public int PageSize { get => pageSize; set => pageSize = value; }
        public int CurrentPage { get => currentPage; set => currentPage = value; }
        public List<Member> Members { get => memberList; set => memberList = value; }
        public int MemberCount { get => memberCount; set => memberCount = value; }

        public String SortOrder { get => sortOrder; set => sortOrder = value; }
        public String CurrentSearchString { get => currentSearchString; set => currentSearchString = value; }
        public String CurrentSearchField { get => currentSearchField; set => currentSearchField = value; }
        public List<String> SearchFieldList { get => searchFieldList; set => searchFieldList = value; }

        public Dictionary<string, string> NameVar { get; set; }

        public static Boolean IsAddMember = false;

        public MemberList()
        {
            this.pageSize = 10;
            this.currentPage = 1;
            this.searchFieldList = new List<String> { "LabID", "Name", "Sex", "Birthday", "Gen", "Unit", "Position"};
            this.NameVar = new Dictionary<string, string>();
            this.NameVar.Add("LabID", "LabID");
            this.NameVar.Add("Name", "Tên thành viên");
            this.NameVar.Add("Sex", "Giới tính");
            this.NameVar.Add("Birthday", "Ngày sinh");
            this.NameVar.Add("Gen", "Thế hệ");
            this.NameVar.Add("Unit", "Đơn vị");
            this.NameVar.Add("Position", "Chức vụ");
        }

        public void Paging(List<Member> members, int pageSize)
        {
            this.memberList = members;
            this.memberCount = members.Count;
            this.pageSize = pageSize;
            
            if ((double)((decimal)this.Members.Count() % Convert.ToDecimal(this.pageSize)) == 0)
            {
                this.pageCount = this.Members.Count() / this.pageSize;
            }
            else
            {
                double page_Count = (double)((decimal)this.Members.Count() / Convert.ToDecimal(this.pageSize));
                this.pageCount = (int)Math.Ceiling(page_Count);
            }

        }
    }
}
