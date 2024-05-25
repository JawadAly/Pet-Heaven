using Pet_Adoption_System.DbConnection;
using Pet_Adoption_System.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebSockets;

namespace Pet_Adoption_System.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        //[Authorize]
        SqlConnection conn;
        SqlCommand sqcmd;
        ConnectionProvider provider;
        List<Category> categoriesList;
        List<Color> colorsList;
        List<Pet> PetList;
        List<foundPet> foundPetList;
        ColorAndCategoryComposite compositeClass;
        List<Adoption> vrfAdoptions;
        List<Adoption> unVrfAdoptions;
        List<Review> reviewList;
        int categoriesCount;
        int petCount;
        public AdminController() {
            provider = new ConnectionProvider();
        }
        public ActionResult Index()
        {
            fetchCategories();
            fetchPets();
            ViewBag.categCount = categoriesCount;
            ViewBag.PetCount = petCount;
            return View();
        }
        public ActionResult Categories() {
            
            fetchCategories();
            return View(categoriesList);  
        }
        [HttpPost]
        public ActionResult Categories(Category category)
        {
            try { 
                //image work here
                string dirPath = Server.MapPath("~/images");
                string imgName = Path.GetFileName(category.file.FileName);
                string completePath = Path.Combine(dirPath, imgName);
                category.file.SaveAs(completePath);

                conn = provider.getConnection();
                conn.Open();
                sqcmd = new SqlCommand("INSERT INTO categories VALUES (@TITLE,@DESC,@IMGURL,@TIMESTAMP)", conn);
                sqcmd.Parameters.AddWithValue("@TITLE", category.categoryTitle);
                sqcmd.Parameters.AddWithValue("@DESC", category.categoryDescription);
                sqcmd.Parameters.AddWithValue("@TIMESTAMP", DateTime.Now);
                sqcmd.Parameters.AddWithValue("@IMGURL",imgName);
                sqcmd.ExecuteNonQuery();
                TempData["message"] = "<script> alert('Category Added Successfully!')  <script>";
                conn.Close();
                //updating categories at view after insertion
                fetchCategories();
            }
            catch (Exception ex)
            {
                TempData["message"] = "<script>alert('Oops unexpected error occured!'+'" + ex.Message + "')</script>";
            }

            return View();
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
                    categ.categoryDescription = sdr["categDesc"].ToString();
                    categ.categoryImage = sdr["categTitleImg"].ToString();
                    categoriesList.Add(categ);
                }
                categoriesCount = categoriesList.Count;
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
                    clr.colorName= sdr["color"].ToString();
                    colorsList.Add(clr);
                }
            }
            conn.Close();
        }
        public ActionResult Pets()
        {
            fetchColors();
            fetchCategories();
            fetchPets();
            compositeClass = new ColorAndCategoryComposite();
            compositeClass.catgrsList = categoriesList;
            compositeClass.clrsList = colorsList;
            compositeClass.petList = PetList;
            return View(compositeClass);
        }
        [HttpPost]
        public ActionResult Pets(Pet pet)
        {
            //image work here
            string dirPath = Server.MapPath("~/images");
            string imgName = Path.GetFileName(pet.file1.FileName);
            string completePath = Path.Combine(dirPath, imgName);
            pet.file1.SaveAs(completePath);

            //image work here
            string imgName1 = Path.GetFileName(pet.file2.FileName);
            string completePath1 = Path.Combine(dirPath, imgName1);
            pet.file2.SaveAs(completePath1);

            //image work here
            string imgName2 = Path.GetFileName(pet.file3.FileName);
            string completePath2 = Path.Combine(dirPath, imgName2);
            pet.file1.SaveAs(completePath2);

            //image work here
            string imgName3 = Path.GetFileName(pet.file4.FileName);
            string completePath3 = Path.Combine(dirPath, imgName3);
            pet.file1.SaveAs(completePath3);

            conn = provider.getConnection();
            conn.Open();
            //sqcmd = new SqlCommand("INSERT INTO pets VALUES (@NAME,@AGE,@TITLEIMG,@IMG2,@IMG3,@IMG4,@TYPE,@STATUS,@COST,@CATEGID,@PETDESC)", conn);
            sqcmd = new SqlCommand("INSERT INTO pets VALUES (@NAME,@AGE,@TITLEIMG,@IMG2,@IMG3,@IMG4,@TYPE,@STATUS,@COST,@CATEGID,@PETDESC);SELECT SCOPE_IDENTITY()", conn);
            sqcmd.Parameters.AddWithValue("@NAME", pet.petName);
            sqcmd.Parameters.AddWithValue("@AGE", pet.petAge);
            sqcmd.Parameters.AddWithValue("@TITLEIMG", imgName);
            sqcmd.Parameters.AddWithValue("@IMG2", imgName1);
            sqcmd.Parameters.AddWithValue("@IMG3", imgName2);
            sqcmd.Parameters.AddWithValue("@IMG4", imgName3);
            sqcmd.Parameters.AddWithValue("@TYPE", 0);
            sqcmd.Parameters.AddWithValue("@STATUS", 1);
            sqcmd.Parameters.AddWithValue("@COST", Convert.ToInt32(pet.petCost));
            sqcmd.Parameters.AddWithValue("@CATEGID", Convert.ToInt32(pet.petCategId));
            sqcmd.Parameters.AddWithValue("@PETDESC", pet.petDesc);
            //sqcmd.ExecuteNonQuery();
            object newPetId = sqcmd.ExecuteScalar();
            conn.Close();
            insertPetColors(Convert.ToInt32(newPetId),pet.petColors);
            return View();
        }
        public void fetchPets()
        {
            try
            {
                PetList = new List<Pet>();
                conn = provider.getConnection();
                conn.Open();
                string query = "SELECT * FROM pets";
                sqcmd = new SqlCommand(query, conn);
                SqlDataReader sdr = sqcmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Pet pet = new Pet();
                        pet.petId = Convert.ToInt32(sdr["petId"]);
                        pet.petName = sdr["petName"].ToString();
                        //pet.petAge = Convert.ToInt32(sdr["petAge"]);
                        pet.petTitleImg = sdr["petTitleImg"].ToString();
                        pet.petStatus = Convert.ToInt32(sdr["petStatus"]);
                        //pet.petCost = Convert.ToInt32(sdr["petCost"]);
                        //pet.petDesc = sdr["petDesc"].ToString();

                        PetList.Add(pet);
                    }
                    petCount = PetList.Count();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                TempData["message"] = "<script> alert('An error has occured on server side')  <script>";
            }

        }
        void insertPetColors(int petId, int[] colors) {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            else {
                    conn = provider.getConnection();
                    conn.Open();
                foreach (var colorId in colors) {
                    sqcmd = new SqlCommand("INSERT INTO petColors VALUES (@PETID,@COLORID)", conn);
                    sqcmd.Parameters.AddWithValue("@PETID",petId);
                    sqcmd.Parameters.AddWithValue("@COLORID", colorId);
                    sqcmd.ExecuteNonQuery();

                }
                    conn.Close();
                
            }
        }
        public ActionResult Verifications() {
            fetchUnverifiedPets();
            return View(foundPetList);
        }
        public void fetchUnverifiedPets() {
            foundPetList = new List<foundPet>();
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("select c.custName,c.custEmail,c.custAddress,c.custPhone,p.petId,p.petName from customersTbl c join CustReportedPets crp on c.custId = crp.customerId join pets p on crp.petId = p.petId where p.petStatus = 0", conn);
            SqlDataReader sdr = sqcmd.ExecuteReader();
            if (sdr.HasRows) {
                while (sdr.Read()) {
                    foundPet fp = new foundPet();
                    fp.pet = new Pet();
                    fp.customer = new Customer();
                    fp.pet.petId = Convert.ToInt32(sdr["petId"]);
                    fp.pet.petName = sdr["petName"].ToString();
                    fp.customer.custName = sdr["custName"].ToString();
                    fp.customer.custEmail = sdr["custEmail"].ToString();
                    fp.customer.custAddress = sdr["custAddress"].ToString();
                    fp.customer.custPhone = sdr["custPhone"].ToString();
                    foundPetList.Add(fp);
                }
            }

        }
        public ActionResult VerifyPet(int id) {
            //first checking for pet existence
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("SELECT petName FROM pets WHERE petId = @ID;",conn);
            sqcmd.Parameters.AddWithValue("@ID",id);
            object val = sqcmd.ExecuteScalar();
            if (val != null)
            {
                UpdateStatus(id);
            }
            else {
                TempData["message"] = "<script> alert('No such pet found!')  </script>";
            }
            conn.Close();
            return RedirectToAction("Verifications");
        }
        void UpdateStatus(int id) {
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("UPDATE pets SET petStatus = 1 WHERE petId = @Id", conn);
            sqcmd.Parameters.AddWithValue("@Id",id);
            sqcmd.ExecuteNonQuery();
            conn.Close();
            TempData["message"] = "<script> alert('Pet Status updated successfully!')  </script>";
        }
        public ActionResult DeletePet(int id) {
            //first checking for pet existence
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("SELECT petName FROM pets where petId = @petId", conn);
            sqcmd.Parameters.AddWithValue("@petId", id);
            object result = sqcmd.ExecuteScalar();
            if (result != null)
            {
                struckPet(id);
            }
            else {
                TempData["message"] = "<script> alert('No such pet found!')  </script>";
            }
            
            conn.Close();
            return RedirectToAction("Verifications");
        }
        void struckPet(int id) {
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("deleteFoundPet", conn);
            sqcmd.Parameters.AddWithValue("@petId",id);
            sqcmd.CommandType = CommandType.StoredProcedure;
            sqcmd.ExecuteNonQuery();
            TempData["message"] = "<script> alert('Pet deleted successfully!')  </script>";
            conn.Close();
        }
        public ActionResult Adoptions() {
            fetchVerifiedAdoptions();
            fetchUnverifiedAdoptions();
            adoptionsViewModel viewModel = new adoptionsViewModel();
            viewModel.vrfAdoptionsList = vrfAdoptions;
            viewModel.unVrfAdoptionsList= unVrfAdoptions;
            return View(viewModel);
        }
        void fetchUnverifiedAdoptions() {
            unVrfAdoptions = new List<Adoption>();
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("select c.custName,c.custPhone,p.PetName,p.petAge,p.petTitleImg,a.adp_id,a.adp_type,a.req_submitted_at,pm.amount,pm.paymentScreenshot from customersTbl c join adoptions a on c.custId = a.cust_id join pets p on p.petId = a.pet_id join payments pm on pm.payment_id = a.payment_id where a.adp_status = 0", conn);
            SqlDataReader sdr = sqcmd.ExecuteReader();
            if (sdr.HasRows) {
                while (sdr.Read()) {
                    Adoption adp = new Adoption();
                    adp.customer = new Customer();
                    adp.pet = new Pet() ;
                    adp.adoptionPayment = new Payment();
                    adp.adoptionId = Convert.ToInt32(sdr["adp_id"]);
                    adp.adoptionType = Convert.ToInt32(sdr["adp_type"]);
                    adp.requestSubmittedAt = Convert.ToDateTime(sdr["req_submitted_at"]);
                    adp.customer.custName = sdr["custName"].ToString();
                    adp.customer.custPhone = sdr["custPhone"].ToString();
                    adp.pet.petName = sdr["PetName"].ToString();
                    adp.pet.petAge = Convert.ToInt32(sdr["petAge"]);
                    adp.pet.petTitleImg = sdr["petTitleImg"].ToString();
                    adp.adoptionPayment.amount = Convert.ToInt32(sdr["amount"]);
                    adp.adoptionPayment.paymentScreenshot = sdr["paymentScreenshot"].ToString();
                    unVrfAdoptions.Add(adp);
                }
            }
            conn.Close();
        }
        void fetchVerifiedAdoptions() {
            vrfAdoptions = new List<Adoption>();
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("select c.custName,c.custPhone,p.PetName,p.petAge,p.petTitleImg,a.adp_id,a.adp_type,a.req_submitted_at,pm.amount,pm.paymentScreenshot from customersTbl c join adoptions a on c.custId = a.cust_id join pets p on p.petId = a.pet_id join payments pm on pm.payment_id = a.payment_id where a.adp_status = 1 order by a.adp_id desc", conn);
            SqlDataReader sdr = sqcmd.ExecuteReader();
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    Adoption adp = new Adoption();
                    adp.customer = new Customer();
                    adp.pet = new Pet();
                    adp.adoptionPayment = new Payment();
                    adp.adoptionId = Convert.ToInt32(sdr["adp_id"]);
                    adp.adoptionType = Convert.ToInt32(sdr["adp_type"]);
                    adp.requestSubmittedAt = Convert.ToDateTime(sdr["req_submitted_at"]);
                    adp.customer.custName = sdr["custName"].ToString();
                    adp.customer.custPhone = sdr["custPhone"].ToString();
                    adp.pet.petName = sdr["PetName"].ToString();
                    adp.pet.petAge = Convert.ToInt32(sdr["petAge"]);
                    adp.pet.petTitleImg = sdr["petTitleImg"].ToString();
                    adp.adoptionPayment.amount = Convert.ToInt32(sdr["amount"]);
                    adp.adoptionPayment.paymentScreenshot = sdr["paymentScreenshot"].ToString();
                    vrfAdoptions.Add(adp);
                }
            }
            conn.Close();
        }
        public ActionResult ReviewsVerification()
        {
            fetchunverifiedReview();
            return View(reviewList);
        }

        public void fetchunverifiedReview()
        {
            try
            {
                reviewList = new List<Review>();
                conn = provider.getConnection();
                conn.Open();
                sqcmd = new SqlCommand("select c.custName,u.review,u.rvtime,u.reviewId from customersTbl c join userReviews u on c.custId=u.custId where u.reviewstatus=0", conn);
                SqlDataReader sdr = sqcmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Review rev = new Review();
                        rev.reviewId = Convert.ToInt32(sdr["reviewId"]);
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
        public ActionResult verifyreview(int id)
        {
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("SELECT review FROM userReviews WHERE reviewId=@ID", conn);
            sqcmd.Parameters.AddWithValue("@ID", id);
            object val = sqcmd.ExecuteScalar();
            if (val != null)
            {
                Updatereview(id);

            }
            else
            {
                TempData["message"] = "<script> alert('No such review found!')  </script>";
            }
            conn.Close();
            return RedirectToAction("ReviewsVerification");
        }
        void Updatereview(int id)
        {
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("UPDATE userReviews SET reviewStatus = 1 WHERE reviewId=@Id", conn);
            sqcmd.Parameters.AddWithValue("@Id", id);
            sqcmd.ExecuteNonQuery();
            conn.Close();
            TempData["message"] = "<script> alert('Review Status updated successfully!')  </script>";
        }
        void DeleteReview(int id)
        {
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("DELETE FROM userReviews WHERE reviewId IN ( SELECT u.reviewId FROM customersTbl c JOIN userReviews u ON c.custId = u.custId  WHERE u.reviewstatus = 0 AND u.reviewId = @id);", conn);
            sqcmd.Parameters.AddWithValue("@id", id);
            sqcmd.ExecuteNonQuery();
            conn.Close();
            TempData["message"] = "<script> alert('Review deleted successfully!') </script>";
        }
        public ActionResult DeleteReviews(int id)
        {
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("SELECT review FROM userReviews WHERE reviewId=@id", conn);
            sqcmd.Parameters.AddWithValue("@id", id);
            object val = sqcmd.ExecuteScalar();
            if (val != null)
            {
                DeleteReview(id);

            }
            else
            {
                TempData["message"] = "<script> alert('No such review found!')  </script>";
            }
            conn.Close();
            return RedirectToAction("ReviewsVerification");
        }

    }
}