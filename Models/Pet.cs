using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pet_Adoption_System.Models
{
    public class Pet
    {
        public int petId  {get;set;}
        public string petName {get;set;}
        public int petAge { get; set; }
        public string petTitleImg { get; set; }
        public string petImg2 { get; set; }
        public string petImg3 { get; set; }
        public string petImg4 { get; set; }
        public HttpPostedFileBase file1 { get; set; }
        public HttpPostedFileBase file2 { get; set; }
        public HttpPostedFileBase file3 { get; set; }
        public HttpPostedFileBase file4 { get; set; }
        public int petType { get; set; }
        public int petStatus { get; set; }
        public int petCost { get; set; }
        public int petCategId { get; set; }
        public string petDesc { get; set; }
    }
}