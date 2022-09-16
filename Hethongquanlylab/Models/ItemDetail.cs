using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hethongquanlylab.Models
{
    public class ItemDetail<T>
    {
        private T item;
        private string sessionVar;
        private string fieldVar;

        public T Item { get => item; set => item = value; }
        public string SessionVar { get => sessionVar; set => sessionVar = value; }
        public string FieldVar { get => fieldVar; set => fieldVar = value; }

        public ItemDetail(T item, string sessionVar)
        {
            this.item = item;
            this.sessionVar = sessionVar;
            this.fieldVar = "";
        }
    }
}
