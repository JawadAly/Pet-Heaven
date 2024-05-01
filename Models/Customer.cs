using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pet_Adoption_System.Models
{
    public class Customer
    {
        public int custId { get; set; }
        public string custName { get; set; }
        public string custEmail { get; set; }
        public string custAddress { get; set; }
        public string custPhone { get; set; }
        public int custAge { get; set; }
        public string userName { get; set; }
        public string userPass { get; set; }
        public string userCreatedAt { get; set; }
    }
}