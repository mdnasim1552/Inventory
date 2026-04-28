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
using InventoryEntity.Supplier;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;

namespace Inventory.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string folderName = "SupplierImages";
        private readonly string uploadFolderPath;
        public SupplierController(IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
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
        public async Task<IActionResult> GetSuppliers([FromForm] DataTablesRequest request, [FromForm] SupplierSearch supplierSearch)
        {
            var supplierList = await _unitOfWork.Supplier.GetAllAsync();
            if (!String.IsNullOrEmpty(supplierSearch.Name))
            {
                supplierList = supplierList.Where(b => b.Name.ToLower().Contains(supplierSearch.Name.ToLower()));
            }
            if (!String.IsNullOrEmpty(supplierSearch.Email))
            {
                supplierList = supplierList.Where(b => b.Email.ToLower().Contains(supplierSearch.Email.ToLower()));
            }
            if (!String.IsNullOrEmpty(supplierSearch.PhoneNumber))
            {
                supplierList = supplierList.Where(b => b.PhoneNumber.ToLower().Contains(supplierSearch.PhoneNumber.ToLower()));
            }
            var supplierListDto = _mapper.Map<List<SupplierDto>>(supplierList);
            // apply search if provided
            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                supplierListDto = supplierListDto
                    .Where(p => p.Name.Contains(request.Search.Value, StringComparison.OrdinalIgnoreCase)
                             || p.PhoneNumber.Contains(request.Search.Value, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            var sortColumnIndex = request.Order[0].Column;
            var sortColumnName = request.Columns[sortColumnIndex].Data;
            var sortDirection = request.Order[0].Dir;
            // Apply ordering dynamically
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                supplierListDto = supplierListDto.AsQueryable()
                                         .OrderBy($"{sortColumnName} {sortDirection}")
                                         .ToList();
            }
            var recordsTotal = supplierListDto.Count();
            if (request.Length == -1)
            {
                request.Length = recordsTotal;
            }

            var data = supplierListDto.Skip(request.Start).Take(request.Length).ToList();
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
        public async Task<IActionResult> Create(SupplierDto supplierDto)
        {
            if (ModelState.IsValid)
            {
                supplierDto.Image = await InventoryUtility.UploadFile(supplierDto.SupplierImg, uploadFolderPath, folderName);//await UploadImage(customerDto.CustomerImg);
                var suppliers = _mapper.Map<Supplier>(supplierDto);
                _unitOfWork.Supplier.Add(suppliers);
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
                return PartialView(supplierDto);
            }
            return View(supplierDto);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var suppliers = await _unitOfWork.Supplier.GetByIdAsync(id);
            if (suppliers == null)
            {
                return NotFound();
            }
            var supplierDto = _mapper.Map<SupplierDto>(suppliers);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(supplierDto);
            }
            return View(supplierDto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(SupplierDto supplierDto)
        {
            if (ModelState.IsValid)
            {
                if (supplierDto.SupplierImg != null)
                {
                    if (supplierDto.Image != null)
                    {
                        var imageUrl = supplierDto.Image.TrimStart('/');
                        imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                        if (System.IO.File.Exists(imageUrl))
                        {
                            System.IO.File.Delete(imageUrl);
                            //return Ok("Image deleted successfully.");
                        }
                    }
                    supplierDto.Image = await InventoryUtility.UploadFile( supplierDto.SupplierImg, uploadFolderPath, folderName);//await UploadImage(customerDto.CustomerImg);
                }
                var suppliers = _mapper.Map<Supplier>(supplierDto);
                _unitOfWork.Supplier.Update(suppliers);
                var supplierstatus = await _unitOfWork.SaveAsync();
                if (supplierstatus)
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
                return PartialView(supplierDto);
            }
            return View(supplierDto);
        }
        public async Task<IActionResult> GetSupplierDetails(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var suppliers = await _unitOfWork.Supplier.GetByIdAsync(id);
            if (suppliers == null)
            {
                return NotFound();
            }
            var supplierDto = _mapper.Map<SupplierDto>(suppliers);
            return PartialView("_SupplierView", supplierDto);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _unitOfWork.Supplier.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            // If the brand has an image, delete it
            if (!string.IsNullOrEmpty(supplier.Image))
            {
                var imageUrl = supplier.Image.TrimStart('/');
                imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                if (System.IO.File.Exists(imageUrl))
                {
                    System.IO.File.Delete(imageUrl);
                }
            }

            _unitOfWork.Supplier.Remove(supplier);
            var result = await _unitOfWork.SaveAsync();

            if (result)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Error while deleting the customer" });
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteImage(string imageUrl, int id)
        {
            var supplier = await _unitOfWork.Supplier.GetByIdAsync(id);
            supplier.Image = null;
            _unitOfWork.Supplier.Update(supplier);
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
    }
}
