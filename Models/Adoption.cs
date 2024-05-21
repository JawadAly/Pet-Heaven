using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pet_Adoption_System.Models
{
    public class Adoption
    {
        public int adoptionId { get; set; }
        public int customerId { get; set; }
        public int petId { get; set; }
        public int adoptionType { get; set; }
        public int adoptionCost { get; set; }
        public int adoptionStatus { get; set; }
        public DateTime requestSubmittedAt { get; set; }
        public Payment adoptionPayment { get; set; }
    }
}