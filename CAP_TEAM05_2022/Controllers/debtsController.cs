using CAP_TEAM05_2022.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CAP_TEAM05_2022.Controllers
{
    public class debtsController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: debts
        public ActionResult Index()
        {
            var debts = db.debts.Include(d => d.sale).Include(d => d.user);
            return View(debts.ToList());
        }

        public JsonResult CreateSale_Debit(debt createDebit)
        {

            string message = "Record Saved Successfully ";
            bool status = true;
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
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
