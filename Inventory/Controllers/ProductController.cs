using AutoMapper;
using Inventory.Data;
using Inventory.Extensions;
using Inventory.Models;
using Inventory.UnitOfWork;
using InventoryEntity.Category;
using InventoryEntity.DataTable;
using InventoryEntity.Product;
using InventoryEntity.SubCategory;
using InventoryRDLC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Linq.Dynamic.Core;

namespace Inventory.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IProcessAccess _processAccess;
        private readonly IEnumerable<Category> categoryList = new List<Category>();
        private readonly IEnumerable<SubCategory> subcategoryList = new List<SubCategory>();
        private readonly IEnumerable<Brand> brandList = new List<Brand>();
        private readonly IEnumerable<Unit> unitList=new List<Unit>();
        public ProductController(IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IProcessAccess processAccess)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _processAccess= processAccess;
            categoryList = _unitOfWork.Category.GetAll();
            subcategoryList = _unitOfWork.SubCategory.GetAll();
            brandList=_unitOfWork.Brand.GetAll();
            unitList = _unitOfWork.Unit.GetAll();
        }
        public async Task<IActionResult> Index()
        {
            //var productList = await _unitOfWork.Product.GetAllIncluding(p => p.Category,p=>p.Brand);

            var userID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserID").Value);
            var adminID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "AdminID").Value);
            var productList = await _unitOfWork.Product.GetAllIncluding(p => p.Category, p => p.Brand,p=>p.CreatedByNavigation.Role);
            var userIdList = await _unitOfWork.Credential.GetUserIdListOnParent(adminID);
            //if (User.FindFirst(ClaimTypes.Role)?.Value == Policies.Admin)
            //{
            //    userIdList.Add(adminID);
            //}
            userIdList.Add(adminID);
            productList = productList.Where(p => userIdList.Contains(p.CreatedBy)).ToList();
            //productList = productList.Where(p => allowedUsers.Contains(p.CreatedBy)).ToList();
            ViewData["CategoryList"] = categoryList;
            ViewData["BrandList"] = brandList;
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Index", productList);
            }
            return View(productList);
        }
        [HttpPost]
        public async Task<IActionResult> GetProducts([FromForm] DataTablesRequest request, [FromForm] ProductSearch productSearch)
        {
            var userID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserID").Value);
            var adminID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "AdminID").Value);
            var productList = await _unitOfWork.Product.GetAllIncluding(p => p.Category, p => p.Brand, p => p.CreatedByNavigation.Role);
            var userIdList = await _unitOfWork.Credential.GetUserIdListOnParent(adminID);

            userIdList.Add(adminID);
            productList = productList.Where(p => userIdList.Contains(p.CreatedBy)).ToList();
            // apply search if provided
            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                productList = productList
                    .Where(p => p.Name.Contains(request.Search.Value, StringComparison.OrdinalIgnoreCase)
                             || p.Sku.Contains(request.Search.Value, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            var sortColumnIndex = request.Order[0].Column;
            var sortColumnName = request.Columns[sortColumnIndex].Data;
            var sortDirection = request.Order[0].Dir;
            // Apply ordering dynamically
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                productList = productList.AsQueryable()
                                         .OrderBy($"{sortColumnName} {sortDirection}")
                                         .ToList();
            }
            var recordsTotal = productList.Count();
            if (request.Length == -1)
            {
                request.Length = recordsTotal;
            }
            var data = productList.Skip(request.Start).Take(request.Length)
                    .Select(p => new {
                        p.Id,
                        p.Name,
                        p.Sku,
                        Category = p.Category.Name,
                        Brand = p.Brand.Name,
                        Price = p.Price.ToString("F2"),
                        Unit = p.Unit.ShortName,
                        Quantity = p.Quantity,
                        CreatedBy = p.CreatedByNavigation.Role.Role,
                        p.Image
                    }).ToList();
            var json = JsonConvert.SerializeObject(new {
                draw = request.Draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = data });
            return Ok(json);
            //return Json(new
            //{
            //    draw = 10,
            //    recordsTotal = 10,
            //    recordsFiltered = 10,  // Apply filtered count if you implement search
            //    data = data
            //});
        }

        [HttpPost]
        public async Task<IActionResult> Index(ProductSearch productSearch)
        {
            var productList = await _unitOfWork.Product.GetAllIncluding(p => p.Category, p => p.Brand, p => p.CreatedByNavigation.Role);

            if (productSearch.CategoryId.HasValue)
            {
                productList = productList.Where(p => p.CategoryId == productSearch.CategoryId);
            }

            if (productSearch.SubCategoryId.HasValue)
            {
                productList = productList.Where(p => p.SubCategoryId == productSearch.SubCategoryId);
            }
            if (productSearch.BrandId.HasValue)
            {
                productList = productList.Where(p => p.BrandId == productSearch.BrandId);
            }
            if (productSearch.Min_Price.HasValue)
            {
                productList = productList.Where(p => p.Price >= productSearch.Min_Price);
            }
            if (productSearch.Max_Price.HasValue)
            {
                productList = productList.Where(p => p.Price <= productSearch.Max_Price);
            }

            //ViewData["CategoryId"] = productSearch.CategoryId;
            //ViewData["SubCategoryId"] = productSearch.SubCategoryId;
            //ViewData["BrandId"] = productSearch.BrandId;
            //ViewData["Min_Price"] = productSearch.Min_Price;
            //ViewData["Max_Price"] = productSearch.Max_Price;


            //ViewData["CategoryList"] = categoryList;
            //ViewData["BrandList"] = brandList;
            //if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            //{
            //    return PartialView("Index", productList);
            //}          
            //return View(productList);
            return PartialView("_ProductList", productList);
        }
        public IActionResult Create()
        {
            ViewData["CategoryList"] = categoryList;
            ViewData["SubcategoryList"] = subcategoryList;
            ViewData["BrandList"] = brandList;
            ViewData["UnitList"] = unitList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                var userID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserID").Value);
                productDto.Image = await UploadImage(productDto.ProductImg);
                productDto.CreatedBy= userID;
                var products = _mapper.Map<Product>(productDto);
                _unitOfWork.Product.Add(products);
                var productstatus = await _unitOfWork.SaveAsync();
                if (productstatus)
                {
                    return RedirectToAction("Create");
                }
            }
            ViewData["CategoryList"] = categoryList;
            ViewData["SubcategoryList"] = subcategoryList;
            ViewData["BrandList"] = brandList;
            ViewData["UnitList"] = unitList;
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            return View(productDto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var products = await _unitOfWork.Product.GetAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            ViewData["CategoryList"] = categoryList;
            ViewData["SubcategoryList"] = subcategoryList;
            ViewData["BrandList"] = brandList;
            ViewData["UnitList"] = unitList;
            var productDto = _mapper.Map<ProductDto>(products);
            productDto.Status = productDto.Status.Trim();
            return View(productDto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                if (productDto.ProductImg != null)
                {
                    if (productDto.Image != null)
                    {
                        var imageUrl = productDto.Image.TrimStart('/');
                        imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                        if (System.IO.File.Exists(imageUrl))
                        {
                            System.IO.File.Delete(imageUrl);
                            //return Ok("Image deleted successfully.");
                        }
                    }
                    productDto.Image = await UploadImage(productDto.ProductImg);
                }
                var products = _mapper.Map<Product>(productDto);
                _unitOfWork.Product.Update(products);
                var productstatus = await _unitOfWork.SaveAsync();
                if (productstatus)
                {
                    TempData["UpdateMessage"] = "Update successfully";
                    return RedirectToAction("Index");
                    //return View(brandDto);
                }
            }
            ViewData["CategoryList"] = categoryList;
            ViewData["SubcategoryList"] = subcategoryList;
            ViewData["BrandList"] = brandList;
            ViewData["UnitList"] = unitList;
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            return View(productDto);
        }

        private async Task<string> UploadImage(IFormFile productImg)
        {
            string uniqueFileName = string.Empty;
            if (productImg != null)
            {
                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var imageExtension = Path.GetExtension(productImg.FileName);
                var imageName = Path.GetFileNameWithoutExtension(productImg.FileName).Length > 20 ?
                    Path.GetFileNameWithoutExtension(productImg.FileName).Substring(0, 20) + imageExtension :
                     Path.GetFileNameWithoutExtension(productImg.FileName) + imageExtension;
                if (!allowedImageExtensions.Contains(imageExtension.ToLower()))
                {
                    throw new ArgumentException("Invalid image type.");
                }
                var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ProductImages");
                // Check if the directory exists, if not, create it
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
                // Generate unique file name using current date, time, and a random string
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                var randomSuffix = Guid.NewGuid().ToString("N").Substring(0, 3); // 8 character suffix
                uniqueFileName = $"{timestamp}_{randomSuffix}_{imageName}";

                //uniqueFileName = Guid.NewGuid().ToString() + "_" + BrandImg.FileName;
                var filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    productImg.CopyTo(fileStream);
                }
                uniqueFileName = $"/ProductImages/{uniqueFileName}";
            }
            return uniqueFileName;
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteImage(string imageUrl, int id)
        {
            var product = await _unitOfWork.Product.GetAsync(id);
            product.Image = null;
            _unitOfWork.Product.Update(product);
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
            var product = await _unitOfWork.Product.GetAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // If the brand has an image, delete it
            if (!string.IsNullOrEmpty(product.Image))
            {
                var imageUrl = product.Image.TrimStart('/');
                imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                if (System.IO.File.Exists(imageUrl))
                {
                    System.IO.File.Delete(imageUrl);
                }
            }

            _unitOfWork.Product.Remove(product);
            var result = await _unitOfWork.SaveAsync();

            if (result)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Error while deleting the Product" });
        }

        [HttpGet]
        public async Task<IActionResult> GetSubCategories(int categoryId)
        {
            var subcategoryList = await _unitOfWork.SubCategory.FindAsync(sc => sc.CategoryId == categoryId);
            var subcategoryListDto = _mapper.Map<List<SubCategoryDto>>(subcategoryList);
            //return Ok(JsonConvert.SerializeObject(new { data = subcategoryListDto}));
            return Json(subcategoryListDto);
        }
        public async Task<IActionResult> GetProductDetails(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var products = await _unitOfWork.Product.GetAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            ViewData["CategoryList"] = categoryList;
            ViewData["SubcategoryList"] = subcategoryList;
            ViewData["BrandList"] = brandList;
            ViewData["UnitList"] = unitList;
            var productDto = _mapper.Map<ProductDto>(products);
            productDto.Status = productDto.Status.Trim();
            return PartialView("_ProductView", productDto);   
        }
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                if (productDto.ProductImg != null)
                {
                    if (productDto.Image != null)
                    {
                        var imageUrl = productDto.Image.TrimStart('/');
                        imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                        if (System.IO.File.Exists(imageUrl))
                        {
                            System.IO.File.Delete(imageUrl);
                            //return Ok("Image deleted successfully.");
                        }
                    }
                    productDto.Image = await UploadImage(productDto.ProductImg);
                }
                var products = _mapper.Map<Product>(productDto);
                _unitOfWork.Product.Update(products);
                var productstatus = await _unitOfWork.SaveAsync();
                if (productstatus)
                {
                    return Json(new { success = true,message= "Update successfully" }); // Return JSON with success = true
                    //return View(brandDto);
                }
            }
            ViewData["CategoryList"] = categoryList;
            ViewData["SubcategoryList"] = subcategoryList;
            ViewData["BrandList"] = brandList;
            ViewData["UnitList"] = unitList;
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");

            return PartialView("_ProductView", productDto);
        }
        //[HttpGet("RptRDLCPDF")]
        [HttpGet]
        public async Task<IActionResult> GenerateProductReport(string fileType)
        {            

            try
            {
                string procedureName = "ALL_REPORTS";
                string Calltype = "PRODUCTS_INFO";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@CallType", Calltype)
                };
                var productDt = await _processAccess.GetDataSets(procedureName, parameters);
                              
                for(int i=0;i<productDt.Tables[0].Rows.Count; i++)
                {
                    productDt.Tables[0].Rows[i]["Image"] = new Uri(Path.Combine(_webHostEnvironment.WebRootPath, productDt.Tables[0].Rows[i]["Image"].ToString())).AbsoluteUri ;
                }
                //var isAuthenticated = User.Identity.IsAuthenticated;
                //var username = User.Identity.Name ?? "Guest";
                //var comcodClaim = User.Claims.FirstOrDefault(c => c.Type == "comcod");
                //var comcod = comcodClaim?.Value ?? "3101";
                //Company company = await _loginRepository.GetCompany(comcod);
                //var userList = await _userRepository.GetUserList();
                //string ComLogo= new Uri(Server.MapPath(@"~\Image\LOGO" + comcod + ".jpg")).AbsoluteUri;
                //string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Comp_Logo", $"{comcod}.jpg");
                string companyName = "HD Clinal";
                string companyAddress = "Mohakhali DOHS, Dhaka, Bangladesh";
                string UserName = "Admin";
                string ComLogo = new Uri(Path.Combine(_webHostEnvironment.WebRootPath,"assets", "img", "logo.png")).AbsoluteUri;
                string printdate = System.DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss tt");

                LocalReport Rpt1 = new LocalReport();
                DataTable dt = new DataTable();
                Rpt1.LoadReportDefinition(RPTPathClass.GetReportFilePath("Reports.ProductReport"));
                Rpt1 = Rpt1.SetRDLCReportDatasets(new Dictionary<string, object> { { "DataSet1", productDt.Tables[0] } });
                //Rpt1.DataSources.Add(new ReportDataSource("DataSet1", userList));
                // Rpt1.EnableExternalImages = true;
                Rpt1.EnableExternalImages = true;

                Rpt1.SetParameters(new ReportParameter("CompanyName", companyName));
                Rpt1.SetParameters(new ReportParameter("CompanyAddress", companyAddress));
                Rpt1.SetParameters(new ReportParameter("ComLogo", ComLogo));
                Rpt1.SetParameters(new ReportParameter("ReportTitle", "Products Report"));
                Rpt1.SetParameters(new ReportParameter("ReportFooter", "Printed from Computer Address:"+companyName+", User:"+ UserName+ ", Time:" + printdate));
                //Rpt1.SetParameters(new ReportParameter("printFooter", ASTUtility.Concat(company != null ? company.comnam : "", username, printdate)));

                string reportExtennsion = fileType == "pdf" ? fileType : (fileType== "msword" ? "docx":"xls");
                string reportType= fileType == "pdf" ? fileType : (fileType == "msword" ? "word" : "excel");
                string deviceInfo =
                      @"<DeviceInfo><EmbedFonts>Full</EmbedFonts>" +
                      "  <OutputFormat>" + fileType + "</OutputFormat>" +
                      "</DeviceInfo>";
                byte[] bytes = Rpt1.Render(reportType, deviceInfo);                                                           
                return File(bytes, "application/"+ fileType, $"Report.{reportExtennsion}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
    }
}
