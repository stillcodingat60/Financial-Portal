using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Financial_Portal.Models
{
    public class Category
    {
        public Category()
        {
            this.Transactions = new HashSet<Transaction>();
        }
        public int Id { get; set; }
        public string CName { get; set; }
        public string Type { get; set; }
        public int? HhId { get; set; }

        public virtual HouseHold Hh { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}