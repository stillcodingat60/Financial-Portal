using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Financial_Portal.Models
{
    public class Budget
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string BName { get; set; }
        public int? CatId { get; set; }
        public string Frequency { get; set; }
        public int HhId { get; set; }
        public int BAmount { get; set; }

        public virtual Category Cat { get; set; }
        
    }
}