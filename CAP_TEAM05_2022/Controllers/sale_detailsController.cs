using CAP_TEAM05_2022.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CAP_TEAM05_2022.Controllers
{
    [CustomAuthorize(Roles = "Quản trị viên, Nhân viên")]
    public class sale_detailsController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();
        public sale_detailsController()
        {
            ViewBag.isNewCreate = false;
        }
        // GET: sale_details
        public ActionResult Index()
        {
            ViewBag.isNewCreate = true;
            return View();
        }

        public ActionResult CreateOldOrder()
        {
            return View("Index");
        }
        public ActionResult getCartProduct(int id)
        {
            return Json(db.sale_details.Include(s => s.product).Include(s => s.sale).Where(c => c.sale_id == id).OrderByDescending(c => c.id).Select(x => new
            {
                cartCode = x.product.code,
                cartName = x.product.name,
                cartUnit = x.unit,
                cartQuantity = x.sold,
                cartPrice = x.price / x.sold,
                cartTotal = x.price,
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult FindSaleDetails(int id)
        {
            sale_details details = db.sale_details.Find(id);
            var emp = new sale();
            emp.note = details.product.code + " - " + details.product.name;
            emp.total = details.sold;
            emp.prepayment = details.price / details.sold;
            emp.id = details.id;
            emp.method = details.product_id;
            emp.code = details.unit;
            return Json(emp);
        }

        public ActionResult _TemplateInvoicePreview(int id)
        {
            var customer = db.customers.Find(id);
            var user = db.users.Find(User.Identity.GetUserId());
            ViewBag.User = user;
            return PartialView(customer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
