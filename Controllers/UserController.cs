using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Pet_Adoption_System.DbConnection;
using Pet_Adoption_System.Models;
using System.Xml.Linq;

namespace Pet_Adoption_System.Controllers
{
    public class UserController : Controller
    {
        SqlConnection conn;
        SqlCommand sqcmd;
        ConnectionProvider provider;
        List<Category> categoriesList;
        List<Color> colorsList;
        ColorAndCategoryComposite compositeClass;
        User user;
        List<Adoption> adoptions;
        public UserController() {
            provider = new ConnectionProvider();
        }
        public ActionResult Index()
        {
            user = Session["userInfo"] as User;
            if (user != null && user.userType == 0)
            {
               return View();    
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        public ActionResult SubmitReview() {
            user = Session["userInfo"] as User;
            if (user != null && user.userType == 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }
        [HttpPost]
        public ActionResult SubmitReview(Review reviewParam)
        {
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("INSERT INTO userReviews VALUES (@custID,@rvTime,@review,@reviewStatus)", conn);
            sqcmd.Parameters.AddWithValue("@custID", reviewParam.custID);
            sqcmd.Parameters.AddWithValue("@rvTime", DateTime.Now);
            sqcmd.Parameters.AddWithValue("@review", reviewParam.review);
            sqcmd.Parameters.AddWithValue("@reviewStatus", 0);
            sqcmd.ExecuteNonQuery();
            TempData["message"] = "<script> alert('Review Added Successfully!')  </script>";
            conn.Close();
            //updating review at view after insertion
            return View();
        }
        public ActionResult ReportFoundPet() {
            user = Session["userInfo"] as User;
            if (user != null && user.userType == 0)
            {
                fetchColors();
                fetchCategories();
                compositeClass = new ColorAndCategoryComposite();
                compositeClass.catgrsList = categoriesList;
                compositeClass.clrsList = colorsList;
                return View(compositeClass);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        public ActionResult ReportFoundPet(Pet pet)
        {
            if (pet.petName == string.Empty || pet.petAge == 0 || pet.file1 == null || pet.petCost == 0 || pet.petDesc == string.Empty)
            {
                TempData["message"] = "<script> alert('Please fill out the required fields!')  </script>";
                return RedirectToAction("ReportFoundPet");
            }
            else {
                //image work here
                string imgName = null, imgName1 = null, imgName2 = null, imgName3 = null;
                string dirPath = Server.MapPath("~/images");
                if (pet.file1 != null) {
                    imgName = Path.GetFileName(pet.file1.FileName);
                    string completePath = Path.Combine(dirPath, imgName);
                    pet.file1.SaveAs(completePath);
                }


                //image work here
                if (pet.file2 != null)
                {
                    imgName1 = Path.GetFileName(pet.file2.FileName);
                    string completePath1 = Path.Combine(dirPath, imgName1);
                    pet.file2.SaveAs(completePath1);
                }
                //image work here
                if (pet.file3 != null)
                {
                    imgName2 = Path.GetFileName(pet.file3.FileName);
                    string completePath2 = Path.Combine(dirPath, imgName2);
                    pet.file1.SaveAs(completePath2);
                }

                //image work here
                if (pet.file4 != null)
                {
                    imgName3 = Path.GetFileName(pet.file4.FileName);
                    string completePath3 = Path.Combine(dirPath, imgName3);
                    pet.file1.SaveAs(completePath3);
                }
                conn = provider.getConnection();
                conn.Open();
                sqcmd = new SqlCommand("PetsColorsCustFoundPetsInsert", conn);
                sqcmd.Parameters.AddWithValue("@petName", pet.petName);
                sqcmd.Parameters.AddWithValue("@petAge", pet.petAge);
                sqcmd.Parameters.AddWithValue("@petTitleImg", imgName);
                sqcmd.Parameters.AddWithValue("@petImg2", imgName1 != null ? imgName1:"null");
                sqcmd.Parameters.AddWithValue("@petImg3", imgName2 != null ? imgName1 : "null");
                sqcmd.Parameters.AddWithValue("@petImg4", imgName3 != null ? imgName1 : "null");
                sqcmd.Parameters.AddWithValue("@petType", 1);
                sqcmd.Parameters.AddWithValue("@petStatus", 0);
                sqcmd.Parameters.AddWithValue("@petCost", Convert.ToInt32(pet.petCost));
                sqcmd.Parameters.AddWithValue("@petCategId", Convert.ToInt32(pet.petCategId));
                sqcmd.Parameters.AddWithValue("@petDesc", pet.petDesc);
                sqcmd.Parameters.AddWithValue("@colorId", pet.foundPetColorId);
                sqcmd.Parameters.AddWithValue("@custId",pet.userId);
                sqcmd.CommandType= CommandType.StoredProcedure;
                sqcmd.ExecuteNonQuery();
                conn.Close();
                return View();
            }
            
        }
        public void fetchCategories()
        {
            categoriesList = new List<Category>();
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("spSelectCategory", conn);
            sqcmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader sdr = sqcmd.ExecuteReader();
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    Category categ = new Category();
                    categ.categoryId = Convert.ToInt32(sdr["categId"]);
                    categ.categoryTitle = sdr["categTitle"].ToString();
                    categoriesList.Add(categ);
                }
            }
            conn.Close();
        }
        public void fetchColors()
        {
            colorsList = new List<Color>();
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("SELECT * FROM colors", conn);
            SqlDataReader sdr = sqcmd.ExecuteReader();
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    Color clr = new Color();
                    clr.colorId = Convert.ToInt32(sdr["colorId"]);
                    clr.colorName = sdr["color"].ToString();
                    colorsList.Add(clr);
                }
            }
            conn.Close();
        }
        public ActionResult MyAdoptions() {
            user = Session["userInfo"] as User;
            if (user != null && user.userType == 0)
            {
                //Customer cust = Session["userInfo"] as Customer;
                fetchmyadoption(user.userId);
                return View(adoptions);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        public void fetchmyadoption(int id)
        {
            adoptions = new List<Adoption>();
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("select p.petName,p.petAge,p.petTitleImg,p.petId,a.adp_type,a.adp_status,a.req_submitted_at,a.cust_id from pets p join adoptions a on p.petId = a.pet_id where a.cust_id = @id", conn);
            sqcmd.Parameters.AddWithValue("@id", id);
            SqlDataReader sdr = sqcmd.ExecuteReader();
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    Adoption adp = new Adoption();
                    adp.customer = new Customer();
                    adp.pet = new Pet();
                    adp.pet.petName = sdr["PetName"].ToString();
                    adp.pet.petAge = Convert.ToInt32(sdr["petAge"]);
                    adp.pet.petTitleImg = sdr["petTitleImg"].ToString();
                    adp.pet.petId = Convert.ToInt32(sdr["petId"]);
                    adp.adoptionType = Convert.ToInt32(sdr["adp_type"]);
                    adp.adoptionStatus = Convert.ToInt32(sdr["adp_status"]);
                    adp.requestSubmittedAt = Convert.ToDateTime(sdr["req_submitted_at"]);
                    adp.customer.custId = Convert.ToInt32(sdr["cust_id"]);

                    adoptions.Add(adp);

                }
            }
            conn.Close();
        }
    }
}