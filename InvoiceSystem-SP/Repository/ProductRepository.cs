using InvoiceSystem_SP.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace InvoiceSystem_SP.Repository
{
    public class ProductRepository
    {
        private readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["InvoiceDBConnectionString"].ConnectionString;

        public int InsertProduct(Product product)
        {
            int newProductId = 0;

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_InsertProduct", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@Price", product.Price);

                con.Open();

                object result = cmd.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int id))
                {
                    newProductId = id;
                }
            }

            return newProductId;
        }
    }
}
