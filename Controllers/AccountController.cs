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
using System.Web.ModelBinding;

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
        [HttpPost]
        public ActionResult Login(User user)
        {
            if (user.userName == string.Empty || user.userPass == string.Empty)
            {
                TempData["message"] = "<script> alert('Please fill out the required fields!')  <script>";
            }
            else {
                conn = provider.getConnection();
                conn.Open();
                sqcmd = new SqlCommand("SELECT userId,userName,userPass,userType FROM Users_tbl WHERE userName = @USERNAME AND userPass = @USERPASS", conn);
                sqcmd.Parameters.AddWithValue("@USERNAME",user.userName);
                sqcmd.Parameters.AddWithValue("@USERPASS",user.userPass);
                SqlDataReader sdr = sqcmd.ExecuteReader();
                if (sdr.Read())
                {
                    user.userType = Convert.ToInt32(sdr["userType"]);
                    user.userId = Convert.ToInt32(sdr["userId"]);
                    if (user.userType == 0)
                    {
                        //TempData["userInfo"] = user;
                        Session["userInfo"] = user;
                        return RedirectToAction("Index", "Home");
                    }
                    else {
                        //TempData["userInfo"] = user;
                        Session["userInfo"] = user;
                        return RedirectToAction("Index","Admin");
                    }
                }
                else {
                    TempData["message"] = "<script> alert('Invalid Credentials!')  <script>";
                }
                conn.Close();
            }
            
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

        public ActionResult Logout() {
            Session.Remove("userInfo");
            return RedirectToAction("Index","Home");
        }
    }
}