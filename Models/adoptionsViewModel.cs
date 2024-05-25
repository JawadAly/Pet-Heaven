using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;

namespace Pet_Adoption_System.Models
{
    public class adoptionsViewModel
    {
        public List<Adoption> unVrfAdoptionsList { get; set; }
        public List<Adoption> vrfAdoptionsList { get; set; }
    }
}