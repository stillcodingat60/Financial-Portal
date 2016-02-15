using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Financial_Portal.Models
{
    public class Invite
    {
        public int Id { get; set; }
        public string EmailInvite { get; set; }
        public int HhId { get; set; }
        public int CodeNr { get; set; }

        public virtual HouseHold Cat { get; set; }

    }
}
    