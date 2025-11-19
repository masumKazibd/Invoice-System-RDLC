using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using InvoiceSystem_SP.Models;
using InvoiceSystem_SP.ViewModels;
namespace InvoiceSystem_SP.Repository
{
    public class InvoiceRepository
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["InvoiceDBConnectionString"].ConnectionString;

        public List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetAllCustomers", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Customer customer = new Customer
                            {
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                Name = reader["Name"].ToString(),
                            };
                            customers.Add(customer);
                        }
                    }
                }
            }

            return customers;
        }

        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetAllProducts", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                Name = reader["Name"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"])
                            };
                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }


        public int CreateInvoice(InvoiceCreationViewModel invoiceVM)
        {
            int newInvoiceId = 0;

            DataTable dtDetails = new DataTable("InvoiceDetailType");
            dtDetails.Columns.Add("ProductID", typeof(int));
            dtDetails.Columns.Add("Quantity", typeof(int));
            dtDetails.Columns.Add("UnitPrice", typeof(decimal));
            dtDetails.Columns.Add("SubTotal", typeof(decimal));

            foreach (var item in invoiceVM.LineItems)
            {
                dtDetails.Rows.Add(item.ProductID, item.Quantity, item.UnitPrice, item.SubTotal);
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_CreateInvoice", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CustomerID", invoiceVM.CustomerID);
                    cmd.Parameters.AddWithValue("@InvoiceDate", System.DateTime.Now);

                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@Details", dtDetails);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.InvoiceDetailType";

                    con.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != System.DBNull.Value)
                    {
                        newInvoiceId = Convert.ToInt32(result);
                    }
                }
            }
            return newInvoiceId;
        }

        public List<InvoiceListViewModel> GetInvoicesByCustomer(string searchTerm)
        {
            List<InvoiceListViewModel> invoiceList = new List<InvoiceListViewModel>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetInvoicesByCustomer", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@SearchTerm", (object)searchTerm ?? DBNull.Value); // Null চেক করা

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            InvoiceListViewModel invoice = new InvoiceListViewModel
                            {
                                InvoiceID = Convert.ToInt32(reader["InvoiceID"]),
                                InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                CustomerName = reader["CustomerName"].ToString()
                            };
                            invoiceList.Add(invoice);
                        }
                    }
                }
            }
            return invoiceList;
        }
    }
}