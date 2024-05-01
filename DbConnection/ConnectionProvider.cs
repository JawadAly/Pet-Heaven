using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Windows;

namespace Pet_Adoption_System.DbConnection
{
    public class ConnectionProvider
    {
        public SqlConnection conn;
        public SqlConnection getConnection()
        {
            try {
                conn = new SqlConnection(ConfigurationManager.ConnectionStrings["petHeavenConnection"].ConnectionString);
            }
            catch (Exception e) { 
                conn = null;
                MessageBox.Show(e.ToString());
            }
            return conn;
        }
    }
}