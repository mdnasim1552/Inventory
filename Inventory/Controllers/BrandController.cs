using AutoMapper;
using Inventory.Models;
using Inventory.UnitOfWork;
using InventoryEntity.Brand;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace Inventory.Controllers
{
    public class BrandController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BrandController(IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;

        }
        public async Task<IActionResult> Index()
        {
            var brandList = await _unitOfWork.Brand.GetAllAsync();
            var brandListDto = _mapper.Map<List<BrandDto>>(brandList);

            return View(brandListDto);
        }
        [HttpPost]
        public async Task<IActionResult> Index(BrandSearch brandSearch)
        {
            var brandList = await _unitOfWork.Brand.GetAllAsync();

            if (!String.IsNullOrEmpty(brandSearch.Name))
            {
                brandList = brandList.Where(b => b.Name.ToLower().Contains(brandSearch.Name.ToLower()));
            }

            if (!String.IsNullOrEmpty(brandSearch.Description))
            {
                brandList = brandList.Where(b => b.Description.ToLower().Contains(brandSearch.Description.ToLower()));
            }

            ViewData["SearchName"] = brandSearch.Name;
            ViewData["SearchDescription"] = brandSearch.Description;
            var brandListDto = _mapper.Map<List<BrandDto>>(brandList);
            return View(brandListDto);
        }
        public  IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(BrandDto brandDto)
        {
            if (ModelState.IsValid)
            {
                brandDto.ImageUrl = await UploadImage(brandDto.BrandImg);
                var brands = _mapper.Map<Brand>(brandDto);
                _unitOfWork.Brand.Add(brands);
                var brandstatus = await _unitOfWork.SaveAsync();
                if (brandstatus)
                {
                    return RedirectToAction("Create");
                }
            }
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            return View(brandDto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var brand = await _unitOfWork.Brand.GetAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            var brandDto = _mapper.Map<BrandDto>(brand);

            return View(brandDto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(BrandDto brandDto)
        {
            if (ModelState.IsValid)
            {
                if (brandDto.BrandImg != null)
                {
                    if(brandDto.ImageUrl!=null)
                    {
                        var imageUrl = brandDto.ImageUrl.TrimStart('/');
                        imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                        if (System.IO.File.Exists(imageUrl))
                        {
                            System.IO.File.Delete(imageUrl);
                            //return Ok("Image deleted successfully.");
                        }
                    }                   
                    brandDto.ImageUrl = await UploadImage(brandDto.BrandImg);
                }
                var brands = _mapper.Map<Brand>(brandDto);
                _unitOfWork.Brand.Update(brands);
                var brandstatus = await _unitOfWork.SaveAsync();
                if (brandstatus)
                {
                    TempData["UpdateMessage"] = "Update successfully";
                    return RedirectToAction("Index");
                    //return View(brandDto);
                }
            }
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            return View(brandDto);
        }

        private async Task<string> UploadImage(IFormFile BrandImg)
        {
            string uniqueFileName = string.Empty;
            if (BrandImg != null)
            {
                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var imageExtension = Path.GetExtension(BrandImg.FileName);
                var imageName = Path.GetFileNameWithoutExtension(BrandImg.FileName).Length > 20 ?
                    Path.GetFileNameWithoutExtension(BrandImg.FileName).Substring(0, 20) + imageExtension :
                     Path.GetFileNameWithoutExtension(BrandImg.FileName) + imageExtension;
                if (!allowedImageExtensions.Contains(imageExtension.ToLower()))
                {
                    throw new ArgumentException("Invalid image type.");
                }
                var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "BrandImages");
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
                    BrandImg.CopyTo(fileStream);
                }
                uniqueFileName = $"/BrandImages/{uniqueFileName}";
            }
            return uniqueFileName;
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteImage(string imageUrl, int id)
        {
            var brand = await _unitOfWork.Brand.GetAsync(id);
            brand.Image = null;
            _unitOfWork.Brand.Update(brand);
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
            var brand = await _unitOfWork.Brand.GetAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            // If the brand has an image, delete it
            if (!string.IsNullOrEmpty(brand.Image))
            {
                var imageUrl = brand.Image.TrimStart('/');
                imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                if (System.IO.File.Exists(imageUrl))
                {
                    System.IO.File.Delete(imageUrl);
                }
            }

            _unitOfWork.Brand.Remove(brand);
            var result = await _unitOfWork.SaveAsync();

            if (result)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Error while deleting the brand" });
        }

    }
}
