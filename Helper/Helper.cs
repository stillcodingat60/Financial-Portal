using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
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

    public static class Extension
    {
        public static string GetHouseHoldId(this IIdentity user)  
        {
            var ClaimsUser = (ClaimsIdentity)user;       //the HouseHoldId is saved as part of the cookie - see Identity Models
            var Claim = ClaimsUser.Claims.FirstOrDefault(c => c.Type == "HouseHoldId");
            if (Claim != null)
              return Claim.Value;                       //will have to Convert.ToInt32(Claim.Value)
            else
              return null;                             //remember to convert to int
        }

        public static bool IsInHouseHold(this IIdentity user)
        {
            var cUser = (ClaimsIdentity)user;
            var hid = cUser.Claims.FirstOrDefault(c => c.Type == "HouseHoldId");
            return (hid != null && !string.IsNullOrWhiteSpace(hid.Value));
        }
    }
}