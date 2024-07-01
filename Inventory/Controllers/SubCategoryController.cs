using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    public class SubCategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
