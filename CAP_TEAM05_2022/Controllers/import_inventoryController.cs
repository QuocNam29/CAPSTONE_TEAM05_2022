using CAP_TEAM05_2022.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CAP_TEAM05_2022.Controllers
{
    [CustomAuthorize(Roles = "Quản trị viên, Nhân viên")]
    public class import_inventoryController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();
        public import_inventoryController()
        {
            ViewBag.isCreate = false;
        }
        // GET: import_inventory
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult InventoryList(DateTime? date_start, DateTime? date_end)
        {
            if (date_start == null)
            {
                date_start = DateTime.Now.AddDays((-DateTime.Now.Day) + 1);
            }
            if (date_end == null)
            {
                date_end = DateTime.Now.AddMonths(1).AddDays(-(DateTime.Now.Day));
            }
            var import_inventory = db.import_inventory.Include(i => i.user).Include(i => i.product).Where(s => s.created_at >= date_start && s.created_at <= date_end
                                                    || s.created_at.Value.Day == date_start.Value.Day
                                                    && s.created_at.Value.Month == date_start.Value.Month
                                                    && s.created_at.Value.Year == date_start.Value.Year
                                                    || s.created_at.Value.Day == date_end.Value.Day
                                                    && s.created_at.Value.Month == date_end.Value.Month
                                                    && s.created_at.Value.Year == date_end.Value.Year);
            return PartialView(import_inventory.OrderByDescending(i => i.id).ToList());
        }

        public ActionResult Create()
        {
            ViewBag.Uniform = db.products.OrderByDescending(o => o.category_id).Select(x => new
            {
                Id = x.id,
                Name = (x.category.name + " - " + x.name + " (" + x.unit + (x.unit_swap != null ? "/" + x.quantity_swap + x.unit_swap : "hihi") + ")").ToString()
            });
            ViewBag.Customer = new SelectList(db.customers.Where(c => c.type == 2), "id", "name");
            ViewBag.isCreate = true;
            return View();
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
