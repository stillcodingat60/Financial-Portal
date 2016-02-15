using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Financial_Portal.Models
{
    public class Helper    //called a Helper Class
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public HouseHold GetHousehold(string userid)
        {
            var user = db.Users.Find(userid);    //given the userid, return the user class
            if(user==null)
            {
                return null;
            }
            if(user.HouseHoldId == null)     //we set the HouseHold Id in the user class via IdentityModels
            {
                return null;
            }
            var hh = db.Households.Find(user.HouseHoldId);  //user to HouseHold is defined in IdentityModels

            return hh;                  //in this case, hh is the HouseHold Object
        }
    }
}