using AutoMapper;
using Inventory.Extensions;
using Inventory.Models;
using Inventory.UnitOfWork;
using InventoryEntity.Account;
using InventoryEntity.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace Inventory.Controllers
{
    //[Authorize(Policy = Policies.HumanResource)] 
    [Authorize(Policy = Policies.Admin)]
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
                bool emailExists = await _unitOfWork.Credential.AnyAsync(c => c.Email == userDto.Email);
                if (emailExists)
                {
                    ModelState.AddModelError(string.Empty, "Email already exist.");
                    return View(userDto);
                }
                else
                {
                    //var isAuthenticated = User.Identity.IsAuthenticated;
                    var adminID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "AdminID").Value);
                    userDto.Image = await InventoryUtility.UploadImage(userDto.UserImg, uploadFolderPath, folderName);
                    userDto.Birthday = Convert.ToDateTime(userDto.Birthday).ToString("yyyy-MM-dd");
                    var users = _mapper.Map<Credential>(userDto);
                    users.CreatedOn = DateTime.Now;
                    users.ParentId = adminID;
                    users.Password = BCrypt.Net.BCrypt.HashPassword(users.Password);
                    _unitOfWork.Credential.Add(users);
                    var userstatus = await _unitOfWork.SaveAsync();
                    if (userstatus)
                    {
                        return RedirectToAction("Create");
                    }
                }                
            }
            ViewData["RoleList"] = roleList;
            ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
            return View(userDto);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.DisableInput = true;
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
            ViewBag.DisableInput = true;
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
                userDto.Birthday = Convert.ToDateTime(userDto.Birthday).ToString("yyyy-MM-dd");
                var user = _mapper.Map<Credential>(userDto);
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _unitOfWork.Credential.Update(user);
                var userstatus = await _unitOfWork.SaveAsync();
                if (userstatus)
                {
                    TempData["UpdateMessage"] = "Update successfully";
                    return RedirectToAction("Index");
                    //return View(brandDto);
                }
                else
                {
                    ViewData["RoleList"] = roleList;
                    ModelState.AddModelError(string.Empty, "Fill the form again correctly!");
                    return View(userDto);
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
