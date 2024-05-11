using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pet_Adoption_System.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            //if (Session["userInfo"] != null)
            //{
            //    return View();
            //}
            //else { 
            //    return RedirectToAction("Login", "Account");
            //}
            return View();
        }
        public ActionResult SubmitReview() {
            //if (Session["userInfo"] != null)
            //{
            //    return View();
            //}
            //else { 
            //    return RedirectToAction("Login", "Account");
            //}
            return View();

        }
        public ActionResult ReportFoundPet() {
            //if (Session["userInfo"] != null)
            //{
            //    return View();
            //}
            //else { 
            //    return RedirectToAction("Login", "Account");
            //}
            return View();
        }
        public ActionResult MyAdoptions() {
            return View();
        }
    }
}