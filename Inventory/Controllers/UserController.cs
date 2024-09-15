using AutoMapper;
using Inventory.Extensions;
using Inventory.Models;
using Inventory.UnitOfWork;
using InventoryEntity.Account;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEnumerable<Userrole> roleList = new List<Userrole>();
        public UserController(IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            roleList = _unitOfWork.Userrole.Find(u=>u.Role!="Admin");
        }
        public async Task<IActionResult> Index()
        {
            var userList =await _unitOfWork.Credential.GetAllIncluding(u => u.Role);
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
                var folderName = "UserImages";
                var uploadFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
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
    }
}
