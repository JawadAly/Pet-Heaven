using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pet_Adoption_System.Models
{
    public class foundPet
    {
        public Pet pet { get; set; }
        public Customer customer { get; set; }
    }
}