using System;
using System.Data;
using System.Web.UI;
using Microsoft.Reporting.WebForms;
using InvoiceSystem_SP.Repository;

namespace InvoiceSystem_SP.Reports 
{
    public partial class WeReportHost : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (int.TryParse(Request.QueryString["id"], out int invoiceId) && invoiceId > 0)
                {
                    try
                    {
                        ReportDataAccess dataAccess = new ReportDataAccess();
                        DataSet ds = dataAccess.GetInvoiceReportData(invoiceId);

                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/InvoiceReport.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();

                        ReportDataSource dsHeader = new ReportDataSource("Invoice", ds.Tables["Invoice"]);

                        ReportDataSource dsDetails = new ReportDataSource("InvoiceDetail", ds.Tables["InvoiceDetail"]);

                        ReportViewer1.LocalReport.DataSources.Add(dsHeader);
                        ReportViewer1.LocalReport.DataSources.Add(dsDetails);

                        ReportViewer1.LocalReport.Refresh();
                    }
                    catch (Exception ex)
                    {
                        Response.Write("<h2>Report Loading Error</h2><p>" + ex.Message + "</p>");
                    }
                }
                else
                {
                    Response.Write("Invalid Invoice ID provided.");
                }
            }
        }
    }
}