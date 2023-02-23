using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CAP_TEAM05_2022.Helper;
using CAP_TEAM05_2022.Models;


namespace CAP_TEAM05_2022.Controllers
{
    [CustomAuthorize(Roles = "Quản trị viên, Nhân viên")]
    public class sale_detailsController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: sale_details
        public ActionResult Index()
        {
            var sale_details = db.sale_details.Include(s => s.product).Include(s => s.sale);
            return View(sale_details.ToList());
        }
        public ActionResult getCartProduct(int id)
        {
            return Json(db.sale_details.Include(s => s.product).Include(s => s.sale).Where(c => c.sale_id == id).OrderByDescending(c => c.id).Select(x => new
            {
                cartCode = x.product.code,
                cartName = x.product.name,
                cartUnit = x.unit,
                cartQuantity = x.sold,
                cartPrice = x.price/ x.sold,                
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
        // GET: sale_details/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sale_details sale_details = db.sale_details.Find(id);
            if (sale_details == null)
            {
                return HttpNotFound();
            }
            return View(sale_details);
        }

        // GET: sale_details/Create
        public ActionResult Create()
        {
            ViewBag.product_id = new SelectList(db.products, "id", "code");
            ViewBag.sale_id = new SelectList(db.sales, "id", "code");
            return View();
        }

        // POST: sale_details/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,sale_id,product_id,price,discount,sold,created_at,updated_at,deleted_at")] sale_details sale_details)
        {
            if (ModelState.IsValid)
            {
                db.sale_details.Add(sale_details);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.product_id = new SelectList(db.products, "id", "code", sale_details.product_id);
            ViewBag.sale_id = new SelectList(db.sales, "id", "code", sale_details.sale_id);
            return View(sale_details);
        }

        // GET: sale_details/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sale_details sale_details = db.sale_details.Find(id);
            if (sale_details == null)
            {
                return HttpNotFound();
            }
            ViewBag.product_id = new SelectList(db.products, "id", "code", sale_details.product_id);
            ViewBag.sale_id = new SelectList(db.sales, "id", "code", sale_details.sale_id);
            return View(sale_details);
        }

        // POST: sale_details/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,sale_id,product_id,price,discount,sold,created_at,updated_at,deleted_at")] sale_details sale_details)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sale_details).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.product_id = new SelectList(db.products, "id", "code", sale_details.product_id);
            ViewBag.sale_id = new SelectList(db.sales, "id", "code", sale_details.sale_id);
            return View(sale_details);
        }

        // GET: sale_details/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sale_details sale_details = db.sale_details.Find(id);
            if (sale_details == null)
            {
                return HttpNotFound();
            }
            return View(sale_details);
        }

        // POST: sale_details/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            sale_details sale_details = db.sale_details.Find(id);
            db.sale_details.Remove(sale_details);
            db.SaveChanges();
            return RedirectToAction("Index");
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
