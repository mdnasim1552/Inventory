using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    public class DiagramController : Controller
    {
        public DiagramController()
        {
            
        }
        public async Task<IActionResult> Index()
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Index");
            }
            return View();
        }
    }
}
