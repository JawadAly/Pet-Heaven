using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pet_Adoption_System.Models
{
    public class Review
    {
        public int reviewId { get; set; }
        public int custID { get; set; }
        public string custname { get; set; }
        public DateTime rvTime { get; set; }
        public string review { get; set; }
        public int reviewStatus { get; set; }
    }
}