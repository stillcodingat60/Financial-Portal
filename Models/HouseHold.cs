using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Financial_Portal.Models
{
    public class HouseHold
    {
        public HouseHold()
        {
            this.HAccounts = new HashSet<HouseAccount>();
            this.Categories = new HashSet<Category>();
            this.Budgets = new HashSet<Budget>();
            this.Invites = new HashSet<Invite>();
            this.Users = new HashSet<ApplicationUser>();
        }

        public int Id { get; set; }
        public string HName { get; set; }       

        public virtual ICollection<HouseAccount> HAccounts { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<Invite> Invites { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}
