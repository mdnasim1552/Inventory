using AutoMapper;
using Inventory.Data;
using Inventory.Extensions;
using Inventory.Models;
using Inventory.UnitOfWork;
using InventoryEntity.Product;
using InventoryEntity.Purchase;
using InventoryEntity.SubCategory;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Inventory.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEnumerable<Supplier> supplierList = new List<Supplier>();
        private readonly IEnumerable<Product> productList = new List<Product>();
        private readonly string folderName = "UserImages";
        private readonly string uploadFolderPath;
        public PurchaseController(IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            supplierList = _unitOfWork.Supplier.GetAll();
            productList = _unitOfWork.Product.GetAll();
            uploadFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
        }
        public async Task<IActionResult> Index()
        {
            var purchaseList = await _unitOfWork.Purchase.GetAllAsync();
            ViewData["SupplierList"] = supplierList;
            ViewData["ProductList"] = productList;
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Index", purchaseList);
            }
            return View(purchaseList);
        }
        public IActionResult Create()
        {
            ViewData["SupplierList"] = supplierList;
            ViewData["ProductList"] = productList;
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView();
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(PurchaseDto purchaseDto)
        {     
            if (ModelState.IsValid)
            {
                var purchaseItemListDto = JsonConvert.DeserializeObject<List<PurchaseItemDto>>(purchaseDto.PurchaseItemsJson);
                var purchaseItemList = _mapper.Map<List<PurchaseItem>>(purchaseItemListDto);
                if (purchaseDto.Invoice_File != null)
                {
                    if (purchaseDto.InvoiceFile != null)
                    {
                        var imageUrl = purchaseDto.InvoiceFile.TrimStart('/');
                        imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                        if (System.IO.File.Exists(imageUrl))
                        {
                            System.IO.File.Delete(imageUrl);
                            //return Ok("Image deleted successfully.");
                        }
                    }
                    purchaseDto.InvoiceFile = await InventoryUtility.UploadImage(purchaseDto.Invoice_File, uploadFolderPath, folderName);
                }
                var purchase = _mapper.Map<Purchase>(purchaseDto);
                purchase.PurchaseItems = purchaseItemList;
                _unitOfWork.Purchase.Add(purchase);
                var purchasestatus = await _unitOfWork.SaveAsync();
                if (purchasestatus)
                {
                    TempData["CreatedMessage"] = "Created successfully";
                    return RedirectToAction("Create");
                }
            }
            ViewData["SupplierList"] = supplierList;
            ViewData["ProductList"] = productList;
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(purchaseDto);
            }
            return View(purchaseDto);
        }
    }
}
