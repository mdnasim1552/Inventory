using AutoMapper;
using Inventory.Models;
using Inventory.UnitOfWork;
using InventoryEntity.Brand;
using InventoryEntity.Category;
using InventoryEntity.SubCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace Inventory.Controllers
{
    [Authorize]
    public class SubCategoryController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEnumerable<Category> categoryList=new List<Category>();
        public SubCategoryController(IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            categoryList = _unitOfWork.Category.GetAll();
        }
        public async Task<IActionResult> Index()
        {
            var SubCategoryList = await _unitOfWork.SubCategory.GetAllIncluding(sc=>sc.Category);
            //var CategoryListDto = _mapper.Map<List<CategoryDto>>(CategoryList);
            ViewData["CategoryList"] = categoryList;
            return View(SubCategoryList);
        }
        [HttpPost]
        public async Task<IActionResult> Index(SubCategorySearch subCategorySearch)
        {
            var subcategoryList = await _unitOfWork.SubCategory.GetAllAsync();

            if (subCategorySearch.CategoryId.HasValue)
            {
                subcategoryList = subcategoryList.Where(b => b.CategoryId==subCategorySearch.CategoryId);
            }

            if (subCategorySearch.SubCategoryId.HasValue)
            {
                subcategoryList = subcategoryList.Where(b=>b.Id==subCategorySearch.SubCategoryId);
            }
            if (!String.IsNullOrEmpty(subCategorySearch.SubCategoryCode))
            {
                subcategoryList = subcategoryList.Where(b => b.Code.ToLower().Contains(subCategorySearch.SubCategoryCode.ToLower()));
            }
            ViewData["CategoryId"] = subCategorySearch.CategoryId;
            ViewData["SubCategoryId"] = subCategorySearch.SubCategoryId;
            ViewData["SubCategoryCode"] = subCategorySearch.SubCategoryCode;
            //var subCategoryListDto = _mapper.Map<List<SubCategoryDto>>(subcategoryList);
            ViewData["CategoryList"] = categoryList;
            return View(subcategoryList);
        }
        public async Task<IActionResult> Create()
        {
            ViewData["CategoryList"] = categoryList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(SubCategoryDto subcategoryDto)
        {
            if (ModelState.IsValid)
            {
                subcategoryDto.Image = await UploadImage(subcategoryDto.SubCategoryImg);
                var subcategories = _mapper.Map<SubCategory>(subcategoryDto);
                _unitOfWork.SubCategory.Add(subcategories);
                var status = await _unitOfWork.SaveAsync();
                if (status)
                {
                    return RedirectToAction("Create");
                }
            }
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            ViewData["CategoryList"] = categoryList;
            return View(subcategoryDto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subcategories = await _unitOfWork.SubCategory.GetAsync(id);
            if (subcategories == null)
            {
                return NotFound();
            }
            ViewData["CategoryList"] = categoryList;
            var subcategoryDto = _mapper.Map<SubCategoryDto>(subcategories);

            return View(subcategoryDto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(SubCategoryDto subcategoryDto)
        {
            if (ModelState.IsValid)
            {
                if (subcategoryDto.SubCategoryImg != null)
                {
                    if (subcategoryDto.Image != null)
                    {
                        var imageUrl = subcategoryDto.Image.TrimStart('/');
                        imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                        if (System.IO.File.Exists(imageUrl))
                        {
                            System.IO.File.Delete(imageUrl);
                            //return Ok("Image deleted successfully.");
                        }
                    }
                    subcategoryDto.Image = await UploadImage(subcategoryDto.SubCategoryImg);
                }
                var subcategories = _mapper.Map<SubCategory>(subcategoryDto);
                _unitOfWork.SubCategory.Update(subcategories);
                var subcategorystatus = await _unitOfWork.SaveAsync();
                if (subcategorystatus)
                {
                    TempData["UpdateMessage"] = "Update successfully";
                    return RedirectToAction("Index");
                    //return View(brandDto);
                }
            }
            ViewData["CategoryList"] = categoryList;
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            return View(subcategoryDto);
        }

        private async Task<string> UploadImage(IFormFile SubCategoryImg)
        {
            string uniqueFileName = string.Empty;
            if (SubCategoryImg != null)
            {
                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var imageExtension = Path.GetExtension(SubCategoryImg.FileName);
                var imageName = Path.GetFileNameWithoutExtension(SubCategoryImg.FileName).Length > 20 ?
                    Path.GetFileNameWithoutExtension(SubCategoryImg.FileName).Substring(0, 20) + imageExtension :
                     Path.GetFileNameWithoutExtension(SubCategoryImg.FileName) + imageExtension;
                if (!allowedImageExtensions.Contains(imageExtension.ToLower()))
                {
                    throw new ArgumentException("Invalid image type.");
                }
                var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "SubCategoryImages");
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
                    SubCategoryImg.CopyTo(fileStream);
                }
                uniqueFileName = $"/SubCategoryImages/{uniqueFileName}";
            }
            return uniqueFileName;
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteImage(string imageUrl, int id)
        {
            var subcategory = await _unitOfWork.SubCategory.GetAsync(id);
            subcategory.Image = null;
            _unitOfWork.SubCategory.Update(subcategory);
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
            var subcategory = await _unitOfWork.SubCategory.GetAsync(id);
            if (subcategory == null)
            {
                return NotFound();
            }

            // If the brand has an image, delete it
            if (!string.IsNullOrEmpty(subcategory.Image))
            {
                var imageUrl = subcategory.Image.TrimStart('/');
                imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                if (System.IO.File.Exists(imageUrl))
                {
                    System.IO.File.Delete(imageUrl);
                }
            }

            _unitOfWork.SubCategory.Remove(subcategory);
            var result = await _unitOfWork.SaveAsync();

            if (result)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Error while deleting the SubCategory" });
        }

        public async Task<IActionResult> GetSubCategoryCode()
        {
            var subcategoryList = await _unitOfWork.SubCategory.GetAllAsync();
            if (subcategoryList == null)
            {
                return Json($"SCT001");
            }
            var maxSubCategoryCode = subcategoryList.Any() == true ? subcategoryList.Select(c => int.Parse(c.Code.Substring(3))).Max() + 1 : 1;//c => int.Parse(c.Substring(2))
            return Json($"CT{maxSubCategoryCode:D3}");
        }
        public async Task<IActionResult> GetSubCategoryCodeById(int Id)
        {
            var subcategory = await _unitOfWork.SubCategory.SingleOrDefaultAsync(s=>s.Id==Id);
            if (subcategory == null)
            {
                return Json($"SCT000");
            }
            return Json(subcategory.Code);
        }
    }
}
