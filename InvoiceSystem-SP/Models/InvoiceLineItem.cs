using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceSystem_SP.Models
{
    public class InvoiceLineItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }
}