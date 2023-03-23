using System.Web.Mvc;

namespace CAP_TEAM05_2022.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult NotFound()
        {
            return View("NotFound");
        }
    }
}