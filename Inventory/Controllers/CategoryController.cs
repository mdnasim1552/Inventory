using AutoMapper;
using Inventory.Models;
using Inventory.UnitOfWork;
using InventoryEntity.Brand;
using InventoryEntity.Category;
using InventoryEntity.SubCategory;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEnumerable<Category> categoryList = new List<Category>();
        public CategoryController(IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            categoryList = _unitOfWork.Category.GetAll();
        }
        public async Task<IActionResult> Index()
        {
            var CategoryList = await _unitOfWork.Category.GetAllAsync();
            var CategoryListDto = _mapper.Map<List<CategoryDto>>(CategoryList);
            ViewData["CategoryList"] = categoryList;
            return View(CategoryListDto);
        }
        [HttpPost]
        public async Task<IActionResult> Index(CategorySearch categorySearch)
        {
            var CategoryList = await _unitOfWork.Category.GetAllAsync();
            

            if (categorySearch.CategoryId.HasValue)
            {
                CategoryList = CategoryList.Where(b => b.Id == categorySearch.CategoryId);
            }           
            if (!String.IsNullOrEmpty(categorySearch.CategoryCode))
            {
                CategoryList = CategoryList.Where(b => b.Code.ToLower().Contains(categorySearch.CategoryCode.ToLower()));
            }
            ViewData["CategoryList"] = categoryList;
            ViewData["CategoryId"] = categorySearch.CategoryId;
            ViewData["CategoryCode"] = categorySearch.CategoryCode;
            var categoryListDto = _mapper.Map<List<CategoryDto>>(CategoryList);
            return View(categoryListDto);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto categoryDto)
        {
            if (ModelState.IsValid)
            {
                categoryDto.Image = await UploadImage(categoryDto.CategoryImg);
                var categories = _mapper.Map<Category>(categoryDto);
                _unitOfWork.Category.Add(categories);
                var brandstatus = await _unitOfWork.SaveAsync();
                if (brandstatus)
                {
                    return RedirectToAction("Create");
                }
            }
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            return View(categoryDto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var categories = await _unitOfWork.Category.GetAsync(id);
            if (categories == null)
            {
                return NotFound();
            }

            var categoryDto = _mapper.Map<CategoryDto>(categories);

            return View(categoryDto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CategoryDto categoryDto)
        {
            if (ModelState.IsValid)
            {
                if (categoryDto.CategoryImg != null)
                {
                    if (categoryDto.Image != null)
                    {
                        var imageUrl = categoryDto.Image.TrimStart('/');
                        imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                        if (System.IO.File.Exists(imageUrl))
                        {
                            System.IO.File.Delete(imageUrl);
                            //return Ok("Image deleted successfully.");
                        }
                    }
                    categoryDto.Image = await UploadImage(categoryDto.CategoryImg);
                }
                var categories = _mapper.Map<Category>(categoryDto);
                _unitOfWork.Category.Update(categories);
                var categorystatus = await _unitOfWork.SaveAsync();
                if (categorystatus)
                {
                    TempData["UpdateMessage"] = "Update successfully";
                    return RedirectToAction("Index");
                    //return View(brandDto);
                }
            }
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            return View(categoryDto);
        }

        private async Task<string> UploadImage(IFormFile CategoryImg)
        {
            string uniqueFileName = string.Empty;
            if (CategoryImg != null)
            {
                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var imageExtension = Path.GetExtension(CategoryImg.FileName);
                var imageName = Path.GetFileNameWithoutExtension(CategoryImg.FileName).Length > 20 ?
                    Path.GetFileNameWithoutExtension(CategoryImg.FileName).Substring(0, 20) + imageExtension :
                     Path.GetFileNameWithoutExtension(CategoryImg.FileName) + imageExtension;
                if (!allowedImageExtensions.Contains(imageExtension.ToLower()))
                {
                    throw new ArgumentException("Invalid image type.");
                }
                var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "CategoryImages");
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
                    CategoryImg.CopyTo(fileStream);
                }
                uniqueFileName = $"/CategoryImages/{uniqueFileName}";
            }
            return uniqueFileName;
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteImage(string imageUrl, int id)
        {
            var category = await _unitOfWork.Category.GetAsync(id);
            category.Image = null;
            _unitOfWork.Category.Update(category);
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
            var category = await _unitOfWork.Category.GetAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // If the brand has an image, delete it
            if (!string.IsNullOrEmpty(category.Image))
            {
                var imageUrl = category.Image.TrimStart('/');
                imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                if (System.IO.File.Exists(imageUrl))
                {
                    System.IO.File.Delete(imageUrl);
                }
            }

            _unitOfWork.Category.Remove(category);
            var result = await _unitOfWork.SaveAsync();

            if (result)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Error while deleting the Category" });
        }
        public async Task<IActionResult> GetCategoryCode()
        {
            var categoryList = await _unitOfWork.Category.GetAllAsync();
            if (categoryList ==null)
            {
                return Json($"CT001");
            }
            var maxCategoryCode = categoryList.Any()==true? categoryList.Select(c=> int.Parse(c.Code.Substring(2))).Max()+1:1;//c => int.Parse(c.Substring(2))
            return Json($"CT{maxCategoryCode:D3}");
        }
        public async Task<IActionResult> GetCategoryCodeById(int Id)
        {
            var category = await _unitOfWork.Category.SingleOrDefaultAsync(c => c.Id == Id);
            if (category == null)
            {
                return Json("");
            }
            return Json(category.Code);
        }
    }
}
