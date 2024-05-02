using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pet_Adoption_System.Models
{
    public class User
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string userPass { get; set; }
        public int userType { get; set; }
        public string userCreatedAt { get; set; }
    }
}