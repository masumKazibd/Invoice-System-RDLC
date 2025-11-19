using InvoiceSystem_SP.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace InvoiceSystem_SP.Repository
{
    public class CustomerRepository

    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["InvoiceDBConnectionString"].ConnectionString;

        public int InsertCustomer(Customer customer)
        {
            int newCustomerId = 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertCustomer", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Name", customer.Name);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.AddWithValue("@Phone", customer.Phone);

                    con.Open();

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        newCustomerId = Convert.ToInt32(result);
                    }
                }
            }
            return newCustomerId;
        }

    }
}