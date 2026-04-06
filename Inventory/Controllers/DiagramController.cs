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
        [HttpPost]
        public async Task<IActionResult> SaveDiagram([FromBody] Diagram diagram)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Index");
            }
            return View();
        }
        public class Diagram
        {
            public string diagramJson { get; set; }
            public string thumbnail { get; set; }
        }
    }
}
