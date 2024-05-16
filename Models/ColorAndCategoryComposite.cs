using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pet_Adoption_System.Models
{
    public class ColorAndCategoryComposite
    {
        public List<Category> catgrsList {get;set;}
        public List<Color> clrsList { get;set;}
        public List<Pet> petList { get; set; }
        public List<Review> reviewList { get; set; }
    }
}