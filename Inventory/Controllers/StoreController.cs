using AutoMapper;
using Inventory.Extensions;
using Inventory.Models;
using Inventory.UnitOfWork;
using InventoryEntity.Account;
using InventoryEntity.Brand;
using InventoryEntity.Category;
using InventoryEntity.Customer;
using InventoryEntity.DataTable;
using InventoryEntity.Product;
using InventoryEntity.Store;
using InventoryEntity.Supplier;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;

namespace Inventory.Controllers
{
    [Authorize]
    public class StoreController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string folderName = "StoreImages";
        private readonly string uploadFolderPath;
        public StoreController(IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            uploadFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
        }
        public async Task<IActionResult> Index()
        {
            //var customerList = await _unitOfWork.Customer.GetAllAsync();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Index");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetStores([FromForm] DataTablesRequest request, [FromForm] StoreSearch storeSearch)
        {
            var storeList = await _unitOfWork.Store.GetAllAsync();
            if (!String.IsNullOrEmpty(storeSearch.Name))
            {
                storeList = storeList.Where(b => b.Name.ToLower().Contains(storeSearch.Name.ToLower()));
            }
            if (!String.IsNullOrEmpty(storeSearch.Code))
            {
                storeList = storeList.Where(b => b.Code.ToLower().Contains(storeSearch.Code.ToLower()));
            }
            if (!String.IsNullOrEmpty(storeSearch.PhoneNumber))
            {
                storeList = storeList.Where(b => b.Phone.ToLower().Contains(storeSearch.PhoneNumber.ToLower()));
            }
            var storeListDto = _mapper.Map<List<StoreDto>>(storeList);
            // apply search if provided
            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                storeListDto = storeListDto
                    .Where(p => p.Name.Contains(request.Search.Value, StringComparison.OrdinalIgnoreCase)
                             || p.Phone.Contains(request.Search.Value, StringComparison.OrdinalIgnoreCase)
                             || p.Code.Contains(request.Search.Value, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            var sortColumnIndex = request.Order[0].Column;
            var sortColumnName = request.Columns[sortColumnIndex].Data;
            var sortDirection = request.Order[0].Dir;
            // Apply ordering dynamically
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                storeListDto = storeListDto.AsQueryable()
                                         .OrderBy($"{sortColumnName} {sortDirection}")
                                         .ToList();
            }
            var recordsTotal = storeListDto.Count();
            if (request.Length == -1)
            {
                request.Length = recordsTotal;
            }

            var data = storeListDto.Skip(request.Start).Take(request.Length).ToList();
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
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView();
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(StoreDto storeDto)
        {
            if (ModelState.IsValid)
            {
                //storeDto.Image = await InventoryUtility.UploadImage(storeDto.StoreImg, uploadFolderPath, folderName);//await UploadImage(customerDto.CustomerImg);
                var stores = _mapper.Map<Store>(storeDto);
                _unitOfWork.Store.Add(stores);
                var supplierstatus = await _unitOfWork.SaveAsync();
                if (supplierstatus)
                {
                    TempData["CreatedMessage"] = "Created successfully";
                    return RedirectToAction("Create");
                }
            }
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(storeDto);
            }
            return View(storeDto);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var stores = await _unitOfWork.Store.GetByIdAsync(id);
            if (stores == null)
            {
                return NotFound();
            }
            var storeDto = _mapper.Map<StoreDto>(stores);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(storeDto);
            }
            return View(storeDto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(StoreDto storeDto)
        {
            if (ModelState.IsValid)
            {
                //if (supplierDto.SupplierImg != null)
                //{
                //    if (supplierDto.Image != null)
                //    {
                //        var imageUrl = supplierDto.Image.TrimStart('/');
                //        imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                //        if (System.IO.File.Exists(imageUrl))
                //        {
                //            System.IO.File.Delete(imageUrl);
                //            //return Ok("Image deleted successfully.");
                //        }
                //    }
                //    supplierDto.Image = await InventoryUtility.UploadImage( supplierDto.SupplierImg, uploadFolderPath, folderName);//await UploadImage(customerDto.CustomerImg);
                //}
                var stores = _mapper.Map<Store>(storeDto);
                stores.CreatedAt = DateTime.Now;
                _unitOfWork.Store.Update(stores);
                var storestatus = await _unitOfWork.SaveAsync();
                if (storestatus)
                {
                    TempData["UpdateMessage"] = "Update successfully";
                    return RedirectToAction("Index");
                    //return View(brandDto);
                }
            }
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            //return PartialView(productDto);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(storeDto);
            }
            return View(storeDto);
        }
        public async Task<IActionResult> GetStoreDetails(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var stores = await _unitOfWork.Store.GetByIdAsync(id);
            if (stores == null)
            {
                return NotFound();
            }
            var storeDto = _mapper.Map<StoreDto>(stores);
            return PartialView("_StoreView", storeDto);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var store = await _unitOfWork.Store.GetByIdAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            // If the brand has an image, delete it
            //if (!string.IsNullOrEmpty(store.Image))
            //{
            //    var imageUrl = store.Image.TrimStart('/');
            //    imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
            //    if (System.IO.File.Exists(imageUrl))
            //    {
            //        System.IO.File.Delete(imageUrl);
            //    }
            //}

            _unitOfWork.Store.Remove(store);
            var result = await _unitOfWork.SaveAsync();

            if (result)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Error while deleting the customer" });
        }
        [HttpPost]
        public async Task<IActionResult> EditStore(StoreDto storeDto)
        {
            if (ModelState.IsValid)
            {
                
                var stores = _mapper.Map<Store>(storeDto);
                stores.CreatedAt = DateTime.Now;
                _unitOfWork.Store.Update(stores);
                var storestatus = await _unitOfWork.SaveAsync();
                if (storestatus)
                {
                    return Json(new { success = true, message = "Update successfully" }); // Return JSON with success = true
                    //return View(brandDto);
                }
            }
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");

            return PartialView("_StoreView", storeDto);
        }
        //[HttpDelete]
        //public async Task<IActionResult> DeleteImage(string imageUrl, int id)
        //{
        //    var store = await _unitOfWork.Store.GetAsync(id);
        //    store.Image = null;
        //    _unitOfWork.Store.Update(store);
        //    var saveResult = await _unitOfWork.SaveAsync();
        //    if (saveResult)
        //    {
        //        imageUrl = imageUrl.TrimStart('/');
        //        imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
        //        if (System.IO.File.Exists(imageUrl))
        //        {
        //            System.IO.File.Delete(imageUrl);
        //            return Ok("Image deleted successfully.");
        //        }
        //    }
        //    return NotFound("Image not found.");
        //}
    }
}
