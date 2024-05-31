using AutoMapper;
using InventoryEntity.Account;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Configuration;
using Inventory.UnitOfWork;
using System.Security.Claims;
using Inventory.Models;

namespace Inventory.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public AccountController(IMapper mapper, IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment,IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _configuration= configuration;
        }
        public IActionResult Signin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Signin(CredentialDto credentialdto)
        {
            var logindto = credentialdto.login;
            // Clear any existing validation errors for the Login property
            ModelState.Remove(nameof(credentialdto.register));
            if (ModelState.IsValid)
            {
                var userInfo = await _unitOfWork.Credential.SingleOrDefaultAsync(c=>c.Email==logindto.Email && c.Password== logindto.Password);
                if (userInfo != null)
                {

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,userInfo.Name),
                        new Claim(ClaimTypes.Email,userInfo.Email)
                     };
                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);//we can use CookieAuthenticationDefaults.AuthenticationScheme (constant) instead of "MyCookieAuth"
                    ClaimsPrincipal claimprincipal = new ClaimsPrincipal(identity);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = logindto.IsRemember
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimprincipal, authProperties);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login Attempt!");
                    return View(credentialdto);

                }
            }
            return View(credentialdto);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Signin", "Account");
        }
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        public async Task<IActionResult> Signup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Signup(CredentialDto credentialdto)
        {
            var registerdto = credentialdto.register;
            // Clear any existing validation errors for the Login property
            ModelState.Remove(nameof(credentialdto.login));
            if (ModelState.IsValid)
            {
                bool emailExists = await _unitOfWork.Credential.AnyAsync(c => c.Email == registerdto.Email);
                if (emailExists) {
                    ModelState.AddModelError(string.Empty, "Email already exist.");
                    return View("Signin", credentialdto);
                }
                else
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,registerdto.Name),
                        new Claim(ClaimTypes.Email,registerdto.Email)                                  
                     };
                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);//we can use CookieAuthenticationDefaults.AuthenticationScheme (constant) instead of "MyCookieAuth"
                    ClaimsPrincipal claimprincipal = new ClaimsPrincipal(identity);
                    var credentials = _mapper.Map<Credential>(registerdto);
                    _unitOfWork.Credential.Add(credentials);
                    var saveResult = await _unitOfWork.SaveAsync();
                    if (saveResult)
                    {
                        string url = _configuration.GetValue<string>("Urls:LoginUrl");
                        string filepath = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplate\\Welcome.cshtml");
                        string htmlstring = System.IO.File.ReadAllText(filepath);
                        htmlstring = htmlstring.Replace("{{company}}", "Inventory Management");
                        htmlstring = htmlstring.Replace("{{User's Name}}", credentials.Name);
                        htmlstring = htmlstring.Replace("{{Username}}", credentials.Email);
                        htmlstring = htmlstring.Replace("{{Password}}", credentials.Password);//{{lgnurl}}
                        htmlstring = htmlstring.Replace("{{lgnurl}}", url);
                        bool status = await _unitOfWork.EmailSetting.SendEmailAsync(credentials.Email, "Account Created", htmlstring);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimprincipal);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return StatusCode(500, "An error occurred while saving the credential");
                    }
                    
                }
            }
            return View(credentialdto);
        }
    }
}
