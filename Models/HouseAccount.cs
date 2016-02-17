using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Financial_Portal.Models
{
    public class HouseAccount
    {
        public HouseAccount()
        {
            this.Transactions = new HashSet<Transaction>();
        }
        public int Id { get; set; }
        public string HAName { get; set; }
        public int HhId { get; set; }
        public decimal Balance { get; set; }

        public virtual HouseHold Hh { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}