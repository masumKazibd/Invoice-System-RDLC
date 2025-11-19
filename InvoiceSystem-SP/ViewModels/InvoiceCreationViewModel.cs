using InvoiceSystem_SP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceSystem_SP.ViewModels
{
    public class InvoiceCreationViewModel
    {
        public int CustomerID { get; set; }

        public string CustomerName { get; set; }
        public List<InvoiceLineItem> LineItems { get; set; }

        public List<Product> AvailableProducts { get; set; }

        public InvoiceCreationViewModel()
        {
            LineItems = new List<InvoiceLineItem>();
        }
    }
}