using Pet_Adoption_System.DbConnection;
using Pet_Adoption_System.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pet_Adoption_System.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection conn;
        SqlCommand sqcmd;
        ConnectionProvider provider;
        public List<Category> categoriesList;
        public List<Pet> PetList;
        List<Review> reviewList;
        Pet incomingPet;
        public HomeController() {
            provider = new ConnectionProvider();
        }
        public ActionResult Index()
        {
            fetchCategories();
            fetchPets();
            fetchReview();
            ColorAndCategoryComposite composite = new ColorAndCategoryComposite();
            composite.catgrsList = categoriesList;
            composite.petList= PetList;
            composite.reviewList = reviewList;
            return View(composite);
        }

        //public ActionResult Categories()
        //{
        //    fetchCategories();
        //    return View(categoriesList);
        //}

        public void fetchCategories() {
            categoriesList = new List<Category>();
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("spSelectCategory", conn);
            sqcmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader sdr = sqcmd.ExecuteReader();
            if (sdr.HasRows) {
                while (sdr.Read()) { 
                    Category categ = new Category();
                    categ.categoryId = Convert.ToInt32(sdr["categId"]) ;
                    categ.categoryTitle = sdr["categTitle"].ToString();
                    categ.categoryDescription = sdr["categDesc"].ToString();
                    categ.categoryImage = sdr["categTitleImg"].ToString();
                    categoriesList.Add(categ);
                }
            }
            conn.Close();
        }

        //public void fetchPets()
        //{
        //    try
        //    {
        //        PetList = new List<Pet>();
        //        conn = provider.getConnection();
        //        conn.Open();
        //        string query = "select petName,petAge,petTitleImg,petImg2,petImg3,petImg4,petCost,petDesc,color from pets p join petColors pC on pC.petId = p.petId join colors c on pC.colorId = c.colorId";
        //        sqcmd = new SqlCommand(query, conn);
        //        SqlDataReader sdr = sqcmd.ExecuteReader();
        //        if (sdr.HasRows)
        //        {
        //            string latesName;
        //            string lastName = "";
        //            while (sdr.Read())
        //            {
        //                //
        //                latesName = sdr["petName"].ToString();
        //                if (latesName != lastName)
        //                {
        //                    Pet pet = new Pet();
        //                    pet.petName = sdr["petName"].ToString();
        //                    pet.petTitleImg = sdr["petTitleImg"].ToString();
        //                    pet.petStatus = Convert.ToInt32(sdr["petStatus"]);
        //                    pet.petStringColors.Append(sdr["color"].ToString());
        //                }
        //                else {
        //                    pet.petStringColors.Append(sdr["color"].ToString());
        //                }


        //                //

        //                //Pet pet = new Pet();
        //                pet.petName = sdr["petName"].ToString();
        //                //pet.petAge = Convert.ToInt32(sdr["petAge"]);
        //                pet.petTitleImg = sdr["petTitleImg"].ToString();
        //                pet.petStatus = Convert.ToInt32(sdr["petStatus"]);
        //                //pet.petCost = Convert.ToInt32(sdr["petCost"]);
        //                //pet.petDesc = sdr["petDesc"].ToString();

        //                PetList.Add(pet);


        //                lastName = sdr["petName"].ToString();
        //            }
        //        }
        //        conn.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["message"] = "<script> alert('An error has occured on server side')  <script>";
        //    }

        //}

        public void fetchPets()
        {
            try
            {
                PetList = new List<Pet>();
                conn = provider.getConnection();
                conn.Open();
                string query = "SELECT * FROM pets WHERE petStatus = 1";
                sqcmd = new SqlCommand(query, conn);
                SqlDataReader sdr = sqcmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Pet pet = new Pet();
                        pet.petName = sdr["petName"].ToString();
                        //pet.petAge = Convert.ToInt32(sdr["petAge"]);
                        pet.petId = Convert.ToInt32(sdr["petId"]);
                        pet.petTitleImg = sdr["petTitleImg"].ToString();
                        pet.petStatus = Convert.ToInt32(sdr["petStatus"]);
                        pet.petAge = Convert.ToInt32(sdr["petAge"]);
                        //pet.petCost = Convert.ToInt32(sdr["petCost"]);
                        //pet.petDesc = sdr["petDesc"].ToString();

                        PetList.Add(pet);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                TempData["message"] = "<script> alert('An error has occured on server side')  <script>";
            }

        }
        
        public ActionResult CategoryView(int id) {
            //first checking the existence of incoming category
            conn = provider.getConnection() ;
            conn.Open();
            sqcmd = new SqlCommand("SELECT categTitle FROM categories WHERE categId = @INCOMINGID", conn);
            sqcmd.Parameters.AddWithValue("@INCOMINGID", id);
            SqlDataReader sdr = sqcmd.ExecuteReader();
            if (sdr.HasRows)
            {
                fetchCategPets(id);
                return View(PetList);
            }
            else {
                TempData["message"] = "<script> alert('No such category exists!')  </script>";
            }
            conn.Close();
            return View("Index");
        }

        public void fetchCategPets(int id) {
                PetList = new List<Pet>();
                conn = provider.getConnection();
                conn.Open();
                sqcmd = new SqlCommand("SELECT petId,petName,petTitleImg,petAge FROM pets WHERE petCategId = @ID AND petStatus = 1", conn);
                sqcmd.Parameters.AddWithValue("@ID",id);
                SqlDataReader sdr = sqcmd.ExecuteReader();
                if (sdr.HasRows) {
                    while (sdr.Read()) {
                        Pet pet = new Pet();
                        pet.petId = Convert.ToInt32(sdr["petId"]);
                        pet.petName = sdr["petName"].ToString();
                        pet.petTitleImg = sdr["petTitleImg"].ToString();
                        pet.petAge = Convert.ToInt32(sdr["petAge"]);
                        PetList.Add(pet);
                    }
                }
                conn.Close();
        }
        public ActionResult PetView(int id) {
            //first checking for pet existence
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("SELECT petName FROM pets WHERE petId = @INCOMINGPETID",conn);
            sqcmd.Parameters.AddWithValue("@INCOMINGPETID",id);
            object result = sqcmd.ExecuteScalar();
            if (result != null)
            {
                incomingPet = new Pet();
                fetchDetailedPetInfo(id);
                fetchPetColors(id);
                return View(incomingPet);
            }
            else {
                TempData["message"] = "<script> alert('No such pet exists!')  </script>";
            }
            conn.Close();
            return View("Index");
        }
        public void fetchDetailedPetInfo(int id) {
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("SELECT * FROM pets WHERE petId = @ID",conn);
            sqcmd.Parameters.AddWithValue("@ID",id);
            SqlDataReader sdr = sqcmd.ExecuteReader();
            if (sdr.Read()) {
                //incomingPet = new Pet();
                incomingPet.petId = Convert.ToInt32(sdr["petId"]);
                incomingPet.petName = sdr["petName"].ToString();
                incomingPet.petTitleImg = sdr["petTitleImg"].ToString();
                incomingPet.petAge = Convert.ToInt32(sdr["petAge"]);
                incomingPet.petImg2 = sdr["petImg2"].ToString(); 
                incomingPet.petImg3 = sdr["petImg3"].ToString(); 
                incomingPet.petImg4 = sdr["petImg4"].ToString();
                incomingPet.petCost = Convert.ToInt32(sdr["petCost"]);
                incomingPet.petDesc = sdr["petDesc"].ToString();
            }
            conn.Close();
        }
        public void fetchPetColors(int id) {
            List<Models.Color> color = new List<Models.Color>();
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("SELECT color FROM colors c JOIN petColors pC ON c.colorId = pC.colorId WHERE pC.petId = @PETID ", conn);
            sqcmd.Parameters.AddWithValue("@PETID",id);
            SqlDataReader sdr = sqcmd.ExecuteReader();
            if (sdr.HasRows) {
                while (sdr.Read()) {
                    Models.Color incomingColor = new Models.Color();
                    incomingColor.colorName = sdr["color"].ToString();
                    color.Add(incomingColor);
                }
                incomingPet.petColorsList = color;
            }
            conn.Close();
        }
        public ActionResult AdoptPet(int id) {
            if (Session["userInfo"] == null) {
                return RedirectToAction("Register","Account");
            }
            else {
                return RedirectToAction("Index");
            }
        }

        public void fetchReview()
        {
            try
            {
                reviewList = new List<Review>();
                conn = provider.getConnection();
                conn.Open();
                sqcmd = new SqlCommand("select c.custName,u.review,u.rvtime from customersTbl c join userReviews u on c.custId=u.custId where u.reviewstatus=0", conn);
                SqlDataReader sdr = sqcmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Review rev = new Review();
                        rev.custname = sdr["custname"].ToString();
                        rev.review = sdr["review"].ToString();
                        rev.rvTime = Convert.ToDateTime(sdr["rvTime"]);

                        reviewList.Add(rev);
                    }
                }
                conn.Close();
            }
            catch (SqlException ex)
            {
                TempData["message"] = "<script> alert('An error has occured on server side')  <script>";
            }
        }

    }
}