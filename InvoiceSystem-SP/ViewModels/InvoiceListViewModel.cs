using System;
using System.ComponentModel.DataAnnotations;

namespace InvoiceSystem_SP.ViewModels
{
    public class InvoiceListViewModel
    {
        public int InvoiceID { get; set; }

        [Display(Name = "Invoice Date")]
        [DataType(DataType.Date)]
        public DateTime InvoiceDate { get; set; }

        [Display(Name = "Customer ID")]
        public int CustomerID { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }
    }
}