using AutoMapper;
using Inventory.Data;
using Inventory.Extensions;
using Inventory.Models;
using Inventory.UnitOfWork;
using InventoryEntity.DataTable;
using InventoryEntity.Product;
using InventoryEntity.Purchase;
using InventoryEntity.SubCategory;
using InventoryEntity.Supplier;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;

namespace Inventory.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEnumerable<Store> storeList = new List<Store>();
        private readonly IEnumerable<Supplier> supplierList = new List<Supplier>();
        private readonly IEnumerable<Product> productList = new List<Product>();
        private readonly string folderName = "InvoiceFiles";
        private readonly string uploadFolderPath;
        public PurchaseController(IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            storeList = _unitOfWork.Store.GetAll();
            supplierList = _unitOfWork.Supplier.GetAll();
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
        [HttpPost]
        public async Task<IActionResult> GetPurchases([FromForm] DataTablesRequest request, [FromForm] PurchaseSearch purchaseSearch)
        {
            var purchaseList = await _unitOfWork.Purchase.GetAllIncluding(p => p.Supplier);
            if (!String.IsNullOrEmpty(purchaseSearch.SupplierName))
            {
                purchaseList = purchaseList.Where(b => b.Supplier.Name.ToLower().Contains(purchaseSearch.SupplierName.ToLower()));
            }
            if (!String.IsNullOrEmpty(purchaseSearch.InvoiceNo))
            {
                purchaseList = purchaseList.Where(b => b.InvoiceNo != null && b.InvoiceNo.ToLower().Contains(purchaseSearch.InvoiceNo.ToLower()));
            }
            if (purchaseSearch.PurchaseDateFrom is not null)
            {
                purchaseList = purchaseList.Where(b => b.PurchaseDate>= purchaseSearch.PurchaseDateFrom);
            }
            if (purchaseSearch.PurchaseDateTo is not null)
            {
                purchaseList = purchaseList.Where(b => b.PurchaseDate <= purchaseSearch.PurchaseDateTo);
            }
            // apply search if provided
            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                purchaseList = purchaseList
                    .Where(p => p.Supplier.Name.Contains(request.Search.Value, StringComparison.OrdinalIgnoreCase)
                             || (p.InvoiceNo != null && p.InvoiceNo.Contains(request.Search.Value, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }
            var sortColumnIndex = request.Order[0].Column;
            var sortColumnName = request.Columns[sortColumnIndex].Data;
            var sortDirection = request.Order[0].Dir;
            // Apply ordering dynamically
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                purchaseList = purchaseList.AsQueryable()
                                         .OrderBy($"{sortColumnName} {sortDirection}")
                                         .ToList();
            }
            var recordsTotal = purchaseList.Count();
            if (request.Length == -1)
            {
                request.Length = recordsTotal;
            }

            var data = purchaseList.Skip(request.Start).Take(request.Length)
                        .Select(p => new {
                            p.Id,
                            p.InvoiceNo,
                            SupplierName = p.Supplier.Name,
                            p.SubTotal,
                            p.Discount,
                            p.Tax,
                            p.TotalAmount,
                        }).ToList();
            var json = JsonConvert.SerializeObject(new
            {
                draw = request.Draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = data
            });
            return Ok(json);
        }
        public IActionResult Create()
        {
            var model = new PurchaseDto
            {
                PurchaseDate = DateTime.Now
            };
            ViewData["StoreList"] = storeList;
            ViewData["SupplierList"] = supplierList;
            ViewData["ProductList"] = productList;
            ViewData["PurchaseItemList"] = new List<PurchaseItemDto>();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(model);
            }
            return View(model);
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
                    purchaseDto.InvoiceFile = await InventoryUtility.UploadFile(purchaseDto.Invoice_File, uploadFolderPath, folderName);
                }
                var purchase = _mapper.Map<Purchase>(purchaseDto);
                var subTotal = purchaseItemList.Sum(x => x.Quantity * x.UnitCost);
                var tax = purchaseItemList.Sum(x => (x.Tax ?? 0) * x.Quantity * x.UnitCost / 100);
                var discount = purchaseItemList.Sum(x => (x.Discount ?? 0) * x.Quantity * x.UnitCost / 100);

                purchase.SubTotal = subTotal;
                purchase.Tax = tax;
                purchase.Discount = discount;
                purchase.TotalAmount = subTotal + tax - discount;
                purchase.PurchaseItems = purchaseItemList;
                _unitOfWork.Purchase.Add(purchase);
                var purchasestatus = await _unitOfWork.SaveAsync();
                if (purchasestatus)
                {
                    foreach (var item in purchaseItemList)
                    {
                        await UpdateProductStoreFromPurchaseCreate(
                            item.ProductId,
                            purchaseDto.StoreId, // must exist
                            item.Quantity
                        );
                    }
                    TempData["CreatedMessage"] = "Created successfully";
                    return RedirectToAction("Create");
                }
            }
            ViewData["StoreList"] = storeList;
            ViewData["SupplierList"] = supplierList;
            ViewData["ProductList"] = productList;
            ViewData["PurchaseItemList"] = JsonConvert.DeserializeObject<List<PurchaseItemDto>>(purchaseDto.PurchaseItemsJson);
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(purchaseDto);
            }
            return View(purchaseDto);
        }
        public async Task UpdateProductStoreFromPurchaseCreate(int productId, int storeId, int newQty)
        {
            var productStore = await _unitOfWork.ProductStore.FirstOrDefaultAsync(x => x.ProductId == productId && x.StoreId == storeId);

            if (productStore != null)
            {
                // ✅ Update existing
                productStore.Quantity += newQty;
                productStore.UpdatedAt = DateTime.Now;

                _unitOfWork.ProductStore.Update(productStore);
            }
            else
            {
                // ✅ Insert new
                var newProductStore = new ProductStore
                {
                    ProductId = productId,
                    StoreId = storeId,
                    Quantity = newQty,
                    UpdatedAt = DateTime.Now
                };

                await _unitOfWork.ProductStore.AddAsync(newProductStore);
            }

            await _unitOfWork.SaveAsync();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //var purchases = await _unitOfWork.Purchase.GetAsync(id);
            var purchase = await _unitOfWork.Purchase.GetIncluding(
                                p => p.Id == id,
                                p => p.PurchaseItems
                            );
            if (purchase == null)
            {
                return NotFound();
            }
            var purchaseDto = _mapper.Map<PurchaseDto>(purchase);
            var purchaseItemListDto = _mapper.Map<List<PurchaseItemDto>>(purchase.PurchaseItems);
            ViewData["PurchaseItemList"] = purchaseItemListDto;
            ViewData["SupplierList"] = supplierList;
            ViewData["ProductList"] = productList;
            ViewData["StoreList"] = storeList;
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(purchaseDto);
            }
            return View(purchaseDto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(PurchaseDto purchaseDto)
        {
            if (ModelState.IsValid)
            {
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var purchase = await _unitOfWork.Purchase.GetByIdAsync(purchaseDto.Id);
                    if (purchase == null)
                        return NotFound();
                    // 🔥 Get existing purchase (TRACKED)
                    var purchaseItems = await _unitOfWork.PurchaseItem.FindAsync(x => x.PurchaseId == purchaseDto.Id);
                    var oldStoreId = purchase.StoreId;
                    foreach (var item in purchaseItems)
                    {
                        var productStore = await _unitOfWork.ProductStore.FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.StoreId == oldStoreId);

                        if (productStore != null)
                        {
                            productStore.Quantity -= item.Quantity;
                            productStore.UpdatedAt = DateTime.Now;
                        }
                    }

                    _unitOfWork.PurchaseItem.RemoveRange(purchaseItems);

                    var purchaseItemListDto = JsonConvert.DeserializeObject<List<PurchaseItemDto>>(purchaseDto.PurchaseItemsJson);
                    var purchaseItemList = _mapper.Map<List<PurchaseItem>>(purchaseItemListDto);

                    foreach (var item in purchaseItemList)
                    {
                        var productStore = await _unitOfWork.ProductStore
                            .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.StoreId == purchaseDto.StoreId);

                        if (productStore != null)
                        {
                            productStore.Quantity += item.Quantity;
                            productStore.UpdatedAt = DateTime.Now;
                        }
                        else
                        {
                            await _unitOfWork.ProductStore.AddAsync(new ProductStore
                            {
                                ProductId = item.ProductId,
                                StoreId = purchaseDto.StoreId,
                                Quantity = item.Quantity,
                                UpdatedAt = DateTime.Now
                            });
                        }
                    }

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
                        purchaseDto.InvoiceFile = await InventoryUtility.UploadFile(purchaseDto.Invoice_File, uploadFolderPath, folderName);//await UploadImage(customerDto.CustomerImg);
                    }
                    _mapper.Map(purchaseDto, purchase);
                    var subTotal = purchaseItemList.Sum(x => x.Quantity * x.UnitCost);
                    var tax = purchaseItemList.Sum(x => (x.Tax ?? 0) * x.Quantity * x.UnitCost / 100);
                    var discount = purchaseItemList.Sum(x => (x.Discount ?? 0) * x.Quantity * x.UnitCost / 100);

                    purchase.SubTotal = subTotal;
                    purchase.Tax = tax;
                    purchase.Discount = discount;
                    purchase.TotalAmount = subTotal + tax - discount;
                    purchase.PurchaseItems = purchaseItemList;
                    purchase.CreatedAt = DateTime.Now;
                    _unitOfWork.Purchase.Update(purchase);
                    var purchasestatus = await _unitOfWork.SaveAsync();
                    if (purchasestatus)
                    {
                        await transaction.CommitAsync();
                        TempData["UpdateMessage"] = "Update successfully";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        TempData["FailMessage"] = "Update failed";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    TempData["FailMessage"] = "Update failed";
                    return RedirectToAction("Index");
                }
                
            }
            ViewData["StoreList"] = storeList;
            ViewData["SupplierList"] = supplierList;
            ViewData["ProductList"] = productList;
            ViewData["PurchaseItemList"] = JsonConvert.DeserializeObject<List<PurchaseItemDto>>(purchaseDto.PurchaseItemsJson);
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(purchaseDto);
            }
            return View(purchaseDto);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteImage(string imageUrl, int id)
        {
            var purchase = await _unitOfWork.Purchase.GetByIdAsync(id);
            purchase.InvoiceFile = null;
            _unitOfWork.Purchase.Update(purchase);
            var saveResult = await _unitOfWork.SaveAsync();
            if (saveResult)
            {
                imageUrl = imageUrl.TrimStart('/');
                imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                if (System.IO.File.Exists(imageUrl))
                {
                    System.IO.File.Delete(imageUrl);
                    return Ok("Image deleted successfully.");
                }
            }
            return NotFound("Image not found.");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            var purchase = await _unitOfWork.Purchase.GetByIdAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }

            // If the brand has an image, delete it
            if (!string.IsNullOrEmpty(purchase.InvoiceFile))
            {
                var imageUrl = purchase.InvoiceFile.TrimStart('/');
                imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                if (System.IO.File.Exists(imageUrl))
                {
                    System.IO.File.Delete(imageUrl);
                }
            }
            var purchaseItems = await _unitOfWork.PurchaseItem.FindAsync(x => x.PurchaseId == id);
            var oldStoreId = purchase?.StoreId;
            foreach (var item in purchaseItems)
            {
                var productStore = await _unitOfWork.ProductStore.FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.StoreId == oldStoreId);

                if (productStore != null)
                {
                    productStore.Quantity -= item.Quantity;
                    productStore.UpdatedAt = DateTime.Now;
                }
            }

            _unitOfWork.PurchaseItem.RemoveRange(purchaseItems);

            _unitOfWork.Purchase.Remove(purchase);
            var result = await _unitOfWork.SaveAsync();

            if (result)
            {
                await transaction.CommitAsync();
                return Json(new { success = true });
            }
            await transaction.RollbackAsync();
            return Json(new { success = false, message = "Error while deleting the Product" });
        }
        public async Task<IActionResult> GetPurchaseDetails(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //var purchases = await _unitOfWork.Purchase.GetAsync(id);
            var purchase = await _unitOfWork.Purchase.GetIncluding(
                                p => p.Id == id,
                                p => p.Supplier,
                                p => p.PurchaseItems
                            );
            if (purchase == null)
            {
                return NotFound();
            }
            var purchaseDto = _mapper.Map<PurchaseDto>(purchase);
            purchaseDto.SupplierName = purchase.Supplier.Name;
            var purchaseItemListDto = _mapper.Map<List<PurchaseItemDto>>(purchase.PurchaseItems);
            ViewData["PurchaseItemList"] = purchaseItemListDto;
            ViewData["SupplierList"] = supplierList;
            ViewData["ProductList"] = productList;
            ViewData["StoreList"] = storeList;
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_PurchaseView",purchaseDto);
            }
            return View("_PurchaseView",purchaseDto);
        }
    }
}
