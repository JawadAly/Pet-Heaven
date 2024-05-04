using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pet_Adoption_System.Models
{
    public class Category
    {
        public int categoryId { get; set; }
        public string categoryTitle { get; set; }
        public string categoryDescription { get; set; }
        public string categoryImage { get; set; }
        public HttpPostedFileBase file { get; set; }
        public string createdAt { get; set; }

    }
}