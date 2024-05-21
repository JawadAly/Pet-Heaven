using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pet_Adoption_System.Models
{
    public class Payment
    {
        public int paymentId { get; set; }
        public int amount { get; set; }
        public string paymentScreenshot { get; set; }
        public HttpPostedFileBase paymentFile { get; set; }
        public DateTime paymentDateTime { get; set; }
    }
}