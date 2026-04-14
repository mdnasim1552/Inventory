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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;

namespace Inventory.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string folderName = "CustomerImages";
        private readonly string uploadFolderPath;
        public CustomerController(IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
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
        public async Task<IActionResult> GetCustomers([FromForm] DataTablesRequest request, [FromForm] CustomerSearch customerSearch)
        {
            var customerList = await _unitOfWork.Customer.GetAllAsync();
            if (!String.IsNullOrEmpty(customerSearch.Name))
            {
                customerList = customerList.Where(b => b.Name.ToLower().Contains(customerSearch.Name.ToLower()));
            }
            if (!String.IsNullOrEmpty(customerSearch.Email))
            {
                customerList = customerList.Where(b => b.Email.ToLower().Contains(customerSearch.Email.ToLower()));
            }
            if (!String.IsNullOrEmpty(customerSearch.PhoneNumber))
            {
                customerList = customerList.Where(b => b.PhoneNumber.ToLower().Contains(customerSearch.PhoneNumber.ToLower()));
            }
            var customerListDto = _mapper.Map<List<CustomerDto>>(customerList);
            // apply search if provided
            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                customerListDto = customerListDto
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
                customerListDto = customerListDto.AsQueryable()
                                         .OrderBy($"{sortColumnName} {sortDirection}")
                                         .ToList();
            }
            var recordsTotal = customerListDto.Count();
            if (request.Length == -1)
            {
                request.Length = recordsTotal;
            }

            var data = customerListDto.Skip(request.Start).Take(request.Length).ToList();
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
        public async Task<IActionResult> Create(CustomerDto customerDto)
        {
            if (ModelState.IsValid)
            {
                customerDto.Image = await InventoryUtility.UploadFile(customerDto.CustomerImg, uploadFolderPath, folderName);//await UploadImage(customerDto.CustomerImg);
                var customers = _mapper.Map<Customer>(customerDto);
                _unitOfWork.Customer.Add(customers);
                var productstatus = await _unitOfWork.SaveAsync();
                if (productstatus)
                {
                    TempData["CreatedMessage"] = "Created successfully";
                    return RedirectToAction("Create");
                }
            }
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(customerDto);
            }
            return View(customerDto);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var customers = await _unitOfWork.Customer.GetAsync(id);
            if (customers == null)
            {
                return NotFound();
            }
            var customerDto = _mapper.Map<CustomerDto>(customers);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(customerDto);
            }
            return View(customerDto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CustomerDto customerDto)
        {
            if (ModelState.IsValid)
            {
                if (customerDto.CustomerImg != null)
                {
                    if (customerDto.Image != null)
                    {
                        var imageUrl = customerDto.Image.TrimStart('/');
                        imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                        if (System.IO.File.Exists(imageUrl))
                        {
                            System.IO.File.Delete(imageUrl);
                            //return Ok("Image deleted successfully.");
                        }
                    }
                    customerDto.Image = await InventoryUtility.UploadFile(customerDto.CustomerImg, uploadFolderPath, folderName);//await UploadImage(customerDto.CustomerImg);
                }
                var customers = _mapper.Map<Customer>(customerDto);
                _unitOfWork.Customer.Update(customers);
                var customerstatus = await _unitOfWork.SaveAsync();
                if (customerstatus)
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
                return PartialView(customerDto);
            }
            return View(customerDto);
        }
        public async Task<IActionResult> GetCustomerDetails(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var customers = await _unitOfWork.Customer.GetAsync(id);
            if (customers == null)
            {
                return NotFound();
            }
            var customerDto = _mapper.Map<CustomerDto>(customers);
            return PartialView("_CustomerView", customerDto);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _unitOfWork.Customer.GetAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // If the brand has an image, delete it
            if (!string.IsNullOrEmpty(customer.Image))
            {
                var imageUrl = customer.Image.TrimStart('/');
                imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                if (System.IO.File.Exists(imageUrl))
                {
                    System.IO.File.Delete(imageUrl);
                }
            }

            _unitOfWork.Customer.Remove(customer);
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
            var customer = await _unitOfWork.Customer.GetAsync(id);
            customer.Image = null;
            _unitOfWork.Customer.Update(customer);
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
