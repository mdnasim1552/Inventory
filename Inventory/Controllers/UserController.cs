using AutoMapper;
using Inventory.Extensions;
using Inventory.Models;
using Inventory.UnitOfWork;
using InventoryEntity.Account;
using InventoryEntity.Product;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEnumerable<Userrole> roleList = new List<Userrole>();
        private readonly string folderName = "UserImages";
        private readonly string uploadFolderPath;
        public UserController(IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            roleList = _unitOfWork.Userrole.Find(u=>u.Role!="Admin");
            uploadFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
        }
        public async Task<IActionResult> Index()
        {
            var userList =await _unitOfWork.Credential.GetAllIncluding(u => u.Role);
            userList = userList.Where(u => u.Role.Role != "Admin");
            return View(userList);
        }
        public ActionResult Create()
        {
            ViewData["RoleList"] = roleList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserDto userDto)
        {
            if (ModelState.IsValid)
            {
                userDto.Image = await InventoryUtility.UploadImage(userDto.UserImg, uploadFolderPath, folderName);
                var users = _mapper.Map<Credential>(userDto);
                users.CreatedOn = DateTime.Now;
                _unitOfWork.Credential.Add(users);
                var userstatus = await _unitOfWork.SaveAsync();
                if (userstatus)
                {
                    return RedirectToAction("Create");
                }
            }
            ViewData["RoleList"] = roleList;
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            return View(userDto);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _unitOfWork.Credential.GetAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleList"] = roleList;
            var userDto = _mapper.Map<UserDto>(user);
            //productDto.Status = productDto.Status.Trim();
            return View(userDto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserDto userDto)
        {
            if (ModelState.IsValid)
            {
                if (userDto.UserImg != null)
                {
                    if (userDto.Image != null)
                    {
                        var imageUrl = userDto.Image.TrimStart('/');
                        imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                        if (System.IO.File.Exists(imageUrl))
                        {
                            System.IO.File.Delete(imageUrl);
                            //return Ok("Image deleted successfully.");
                        }
                    }
                    userDto.Image = await InventoryUtility.UploadImage(userDto.UserImg, uploadFolderPath, folderName);
                }
                var user = _mapper.Map<Credential>(userDto);
                _unitOfWork.Credential.Update(user);
                var userstatus = await _unitOfWork.SaveAsync();
                if (userstatus)
                {
                    TempData["UpdateMessage"] = "Update successfully";
                    return RedirectToAction("Index");
                    //return View(brandDto);
                }
            }
            ViewData["RoleList"] = roleList;
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            return View(userDto);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _unitOfWork.Credential.GetAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // If the brand has an image, delete it
            if (!string.IsNullOrEmpty(user.Image))
            {
                var imageUrl = user.Image.TrimStart('/');
                imageUrl = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                if (System.IO.File.Exists(imageUrl))
                {
                    System.IO.File.Delete(imageUrl);
                }
            }

            _unitOfWork.Credential.Remove(user);
            var result = await _unitOfWork.SaveAsync();

            if (result)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Error while deleting the User" });
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteImage(string imageUrl, int id)
        {
            var user = await _unitOfWork.Credential.GetAsync(id);
            user.Image = null;
            _unitOfWork.Credential.Update(user);
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
