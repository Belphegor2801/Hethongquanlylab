using System;
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

        public int PageCount { get => pageCount; set => pageCount = value; }
        public int PageSize { get => pageSize; set => pageSize = value; }
        public int CurrentPage { get => currentPage; set => currentPage = value; }
        public List<Member> Members { get => memberList; set => memberList = value; }

        public MemberList(List<Member> members)
        {
            this.memberList = members;
            this.pageSize = 10;
            this.pageCount = members.Count / this.pageSize;
            this.currentPage = 1;
        }
    }
}
