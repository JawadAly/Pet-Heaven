using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using Pet_Adoption_System.DbConnection;
using System.Web.WebSockets;
using Pet_Adoption_System.Models;
using System.Data;

namespace Pet_Adoption_System.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        SqlConnection conn;
        ConnectionProvider provider;
        SqlCommand sqcmd;
        public AccountController() { 
            provider = new ConnectionProvider();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Register() {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Customer cust)
        {
            conn = provider.getConnection();
            conn.Open();
            sqcmd = new SqlCommand("spUsersAndCustomers",conn);
            sqcmd.CommandType = CommandType.StoredProcedure;
            sqcmd.Parameters.AddWithValue("@userName", cust.userName);
            sqcmd.Parameters.AddWithValue("@userPass", cust.userPass);
            sqcmd.Parameters.AddWithValue("@userType", 0);
            sqcmd.Parameters.AddWithValue("@usr_created_at", DateTime.Now);
            sqcmd.Parameters.AddWithValue("@custName", cust.custName);
            sqcmd.Parameters.AddWithValue("@custEmail", cust.custEmail);
            sqcmd.Parameters.AddWithValue("@custAddress", cust.custAddress);
            sqcmd.Parameters.AddWithValue("@custPhone", cust.custPhone);
            sqcmd.Parameters.AddWithValue("@custAge", cust.custAge);
            sqcmd.ExecuteNonQuery();
            conn.Close();
            TempData["message"] = "<script> alert('Customer Registered Successfuly!')  <script>";
            return RedirectToAction("Login");
        }


    }
}