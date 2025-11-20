using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using InvoiceSystem_SP.Models;
using InvoiceSystem_SP.Repository;
using InvoiceSystem_SP.ViewModels;

namespace InvoiceSystem_SP.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly InvoiceRepository invoiceRepository = new InvoiceRepository();

        [HttpGet]
        public ActionResult Create()
        {
            var model = new InvoiceCreationViewModel();

            try
            {
                List<Customer> customers = invoiceRepository.GetAllCustomers();
                model.AvailableProducts = invoiceRepository.GetAllProducts();

                ViewBag.CustomerList = new SelectList(customers, "CustomerID", "Name");

                if (model.LineItems == null)
                {
                    model.LineItems = new List<InvoiceLineItem>();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Failed to load required data: " + ex.Message;
            }

            return View(model);
        }

        // POST: Invoice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InvoiceCreationViewModel invoiceVM)
        {
            if (invoiceVM.LineItems != null)
            {
                invoiceVM.LineItems = invoiceVM.LineItems
                    .Where(li =>
                        li != null &&
                        li.ProductID > 0 &&
                        li.Quantity > 0 &&
                        li.UnitPrice > 0)
                    .ToList();
            }

            // Validation after cleaning
            if (invoiceVM.CustomerID <= 0 ||
                invoiceVM.LineItems == null ||
                !invoiceVM.LineItems.Any())
            {
                ViewBag.ErrorMessage = "Please select a Customer and add at least one valid product line item (Quantity > 0).";

                List<Customer> customers = invoiceRepository.GetAllCustomers();
                ViewBag.CustomerList = new SelectList(customers, "CustomerID", "Name");
                invoiceVM.AvailableProducts = invoiceRepository.GetAllProducts();

                if (invoiceVM.LineItems == null)
                {
                    invoiceVM.LineItems = new List<InvoiceLineItem>();
                }

                return View(invoiceVM);
            }

            try
            {
                foreach (var item in invoiceVM.LineItems)
                {
                    item.SubTotal = item.Quantity * item.UnitPrice;
                }

                int newInvoiceId = invoiceRepository.CreateInvoice(invoiceVM);

                if (newInvoiceId > 0)
                {
                    TempData["SuccessMessage"] = "Invoice created successfully. Invoice ID: " + newInvoiceId;
                    return RedirectToAction("ViewInvoice", new { id = newInvoiceId });
                }

                ViewBag.ErrorMessage = "Invoice creation failed due to a database transaction error.";

                List<Customer> customers = invoiceRepository.GetAllCustomers();
                ViewBag.CustomerList = new SelectList(customers, "CustomerID", "Name");
                invoiceVM.AvailableProducts = invoiceRepository.GetAllProducts();
                return View(invoiceVM);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred: " + ex.Message;

                List<Customer> customers = invoiceRepository.GetAllCustomers();
                ViewBag.CustomerList = new SelectList(customers, "CustomerID", "Name");
                invoiceVM.AvailableProducts = invoiceRepository.GetAllProducts();
                return View(invoiceVM);
            }
        }

        // GET: Invoice/ViewInvoice/{id} 
        public ActionResult ViewInvoice(int id)
        {
            ViewBag.InvoiceID = id;
            return View();
        }

        // GET: Invoice/List
        public ActionResult List(string searchTerm = null)
        {
            List<InvoiceListViewModel> invoiceList = invoiceRepository.GetInvoicesByCustomer(searchTerm);
            ViewBag.CurrentSearch = searchTerm;
            return View(invoiceList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchInvoices(string searchCustomer)
        {
            return RedirectToAction("List", new { searchTerm = searchCustomer });
        }
    }
}
