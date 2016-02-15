using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Financial_Portal.Models
{
    public class ContactMessage
    {

        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Message { get; set; }
        public int HhId { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }

    }
}