using InvoiceSystem_SP.Models;
using InvoiceSystem_SP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceSystem_SP.Controllers
{
    public class CustomerController : Controller
    {
        private CustomerRepository customerRepository = new CustomerRepository();
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer customer)
        {
            int newId = customerRepository.InsertCustomer(customer);

            if (newId > 0)
            {
                TempData["SuccessMessage"] = "Customer created successfully ID: " + newId;
                return RedirectToAction("Index", "Home");
            }
            return View(customer);
        }
    }
}