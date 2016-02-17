using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Financial_Portal.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public int? CatId { get; set; }
        public int? HAccountId { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Descript { get; set; }
        public Boolean Reconcile { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public string Type { get; set; }

        public virtual Category Cat { get; set; }
        public virtual HouseAccount HAccount { get; set; }

    }
}