using System.Web.Mvc;
using System.Collections.Generic;
using InvoiceSystem_SP.Models;
using InvoiceSystem_SP.Repository;
using InvoiceSystem_SP.ViewModels;
using System.Linq;

namespace InvoiceSystem_SP.Controllers
{
    public class InvoiceController : Controller
    {
        private InvoiceRepository invoiceRepository = new InvoiceRepository();
        
        public ActionResult Create()
        {
            InvoiceCreationViewModel model = new InvoiceCreationViewModel();

            try
            {
                List<Customer> customers = invoiceRepository.GetAllCustomers();
                model.AvailableProducts = invoiceRepository.GetAllProducts();

                ViewBag.CustomerList = new SelectList(customers, "CustomerID", "Name");

                model.LineItems.Add(new InvoiceLineItem());
            }
            catch (System.Exception ex)
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
            if (invoiceVM.CustomerID <= 0 ||
                invoiceVM.LineItems == null ||
                !invoiceVM.LineItems.Any(item => item.Quantity > 0))
            {
                ViewBag.ErrorMessage = "Please select a Customer and add at least one valid product line item (Quantity > 0).";

                List<Customer> customers = invoiceRepository.GetAllCustomers();
                ViewBag.CustomerList = new SelectList(customers, "CustomerID", "Name");
                invoiceVM.AvailableProducts = invoiceRepository.GetAllProducts();
                return View(invoiceVM);
            }

            try
            {
                foreach (var item in invoiceVM.LineItems.Where(i => i.Quantity > 0))
                {
                    item.SubTotal = item.Quantity * item.UnitPrice;
                }

                int newInvoiceId = invoiceRepository.CreateInvoice(invoiceVM);

                if (newInvoiceId > 0)
                {
                    TempData["SuccessMessage"] = "Invoice created successfully. Invoice ID: " + newInvoiceId;
                    return RedirectToAction("ViewInvoice", new { id = newInvoiceId });
                }
                else
                {
                    ViewBag.ErrorMessage = "Invoice creation failed due to a database transaction error.";

                    List<Customer> customers = invoiceRepository.GetAllCustomers();
                    ViewBag.CustomerList = new SelectList(customers, "CustomerID", "Name");
                    invoiceVM.AvailableProducts = invoiceRepository.GetAllProducts();
                    return View(invoiceVM);
                }
            }
            catch (System.Exception ex)
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