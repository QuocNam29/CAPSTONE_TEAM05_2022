using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CAP_TEAM05_2022.Helper;
using CAP_TEAM05_2022.Models;

namespace CAP_TEAM05_2022.Controllers
{
    [LoginVerification]
    public class import_inventoryController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: import_inventory
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult InventoryList(DateTime? date_start, DateTime? date_end)
        {
            if (date_start == null)
            {
                date_start =DateTime.Now.AddDays((-DateTime.Now.Day) + 1);
            }
            if (date_end == null)
            {
                date_end = DateTime.Now.AddMonths(1).AddDays(-(DateTime.Now.Day));
            }
            var import_inventory = db.import_inventory.Include(i => i.user).Include(i => i.product).Where(s => s.created_at >= date_start && s.created_at <= date_end
                                            || s.created_at.Value.Day == date_start.Value.Day && s.created_at.Value.Day == date_end.Value.Day);
            return PartialView(import_inventory.OrderByDescending(i => i.id).ToList());
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
