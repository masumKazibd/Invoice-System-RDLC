using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace InvoiceSystem_SP.Repository
{
    public class ReportDataAccess
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["InvoiceDBConnectionString"].ConnectionString;

        public DataSet GetInvoiceReportData(int invoiceId)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetInvoiceDataForReport", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            conn.Open();
                        
                            da.Fill(ds);

                            if (ds.Tables.Count > 0)
                            {
                                ds.Tables[0].TableName = "InvoiceHeader";
                            }
                            if (ds.Tables.Count > 1)
                            {
                                ds.Tables[1].TableName = "InvoiceDetails";
                            }
                        }
                        catch (SqlException ex)
                        {
                            throw new Exception("Database Error fetching report data: " + ex.Message);
                        }
                    }
                }
            }
            return ds;
        }
    }
}