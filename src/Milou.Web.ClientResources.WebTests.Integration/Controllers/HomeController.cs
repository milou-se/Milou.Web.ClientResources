using System.Web.Mvc;

namespace Milou.Web.ClientResources.WebTests.Integration.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}