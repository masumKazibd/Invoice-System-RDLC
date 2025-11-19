using InvoiceSystem_SP.Models;
using InvoiceSystem_SP.Repository;
using System.Web.Mvc;

namespace InvoiceSystem_SP.Controllers
{
    public class ProductController : Controller
    {
        private ProductRepository productRepository = new ProductRepository();
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            int newId = productRepository.InsertProduct(product);

            if (newId > 0)
            {
                TempData["SuccessMessage"] = "Product added successfully ID: " + newId;
                return RedirectToAction("Index", "Home");
            }

            return View(product);
        }
    }
}