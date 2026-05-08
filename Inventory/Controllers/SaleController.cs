using AutoMapper;
using Inventory.Models;
using Inventory.UnitOfWork;
using InventoryEntity.Purchase;
using InventoryEntity.Sale;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    public class SaleController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEnumerable<Product> productList = new List<Product>();
        private readonly string folderName = "InvoiceFiles";
        private readonly string uploadFolderPath;
        public SaleController(IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            productList = _unitOfWork.Product.GetAll();
            uploadFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
        }
        public async Task<IActionResult> Index()
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView();
            }
            return View();
        }
        public IActionResult Create()
        {
            var model = new SaleDto
            {
                SaleDate = DateTime.Now
            };
            ViewData["ProductList"] = productList;
            ViewData["PurchaseItemList"] = new List<PurchaseItemDto>();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(model);
            }
            return View(model);
        }
    }
}
