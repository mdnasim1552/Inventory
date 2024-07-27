using AutoMapper;
using Inventory.Models;
using Inventory.UnitOfWork;
using InventoryEntity.Category;
using InventoryEntity.Product;
using InventoryEntity.SubCategory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Inventory.Controllers
{
    public class ProductController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEnumerable<Category> categoryList = new List<Category>();
        private readonly IEnumerable<SubCategory> subcategoryList = new List<SubCategory>();
        private readonly IEnumerable<Brand> brandList = new List<Brand>();
        private readonly IEnumerable<Unit> unitList=new List<Unit>();
        public ProductController(IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            categoryList = _unitOfWork.Category.GetAll();
            subcategoryList = _unitOfWork.SubCategory.GetAll();
            brandList=_unitOfWork.Brand.GetAll();
            unitList = _unitOfWork.Unit.GetAll();
        }
        public async Task<IActionResult> Index()
        {
            var productList = await _unitOfWork.Product.GetAllIncluding(p => p.Category,p=>p.Brand);
            ViewData["CategoryList"] = categoryList;
            ViewData["BrandList"] = brandList;
            return View(productList);
        }
        [HttpPost]
        public async Task<IActionResult> Index(ProductSearch productSearch)
        {
            var productList = await _unitOfWork.Product.GetAllIncluding(p => p.Category, p => p.Brand);

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

            ViewData["CategoryId"] = productSearch.CategoryId;
            ViewData["SubCategoryId"] = productSearch.SubCategoryId;
            ViewData["BrandId"] = productSearch.BrandId;
            ViewData["Min_Price"] = productSearch.Min_Price;
            ViewData["Max_Price"] = productSearch.Max_Price;


            ViewData["CategoryList"] = categoryList;
            ViewData["BrandList"] = brandList;
            return View(productList);
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
                productDto.Image = await UploadImage(productDto.ProductImg);
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

    }
}
