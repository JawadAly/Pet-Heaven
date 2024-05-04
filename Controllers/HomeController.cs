using Pet_Adoption_System.DbConnection;
using Pet_Adoption_System.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        public HomeController() {
            provider = new ConnectionProvider();
        }
        public ActionResult Index()
        {
            fetchCategories();
            return View(categoriesList);
        }

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
    }
}