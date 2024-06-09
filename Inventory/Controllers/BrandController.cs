using AutoMapper;
using Inventory.Models;
using Inventory.UnitOfWork;
using InventoryEntity.Brand;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

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
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(BrandDto brandDto)
        {
            if (ModelState.IsValid)
            {
                brandDto.ImageUrl =await UploadImage(brandDto.BrandImg);
                var brands=_mapper.Map<Brand>(brandDto);
                _unitOfWork.BrandRepository.Add(brands);
                var brandstatus= await _unitOfWork.SaveAsync();
                if (brandstatus)
                {
                    return RedirectToAction("Index");
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
                uniqueFileName = Guid.NewGuid().ToString() + "_" + BrandImg.FileName;
                var filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    BrandImg.CopyTo(fileStream);
                }
                uniqueFileName = filePath;
            }
            return uniqueFileName;
        }
    }
}
