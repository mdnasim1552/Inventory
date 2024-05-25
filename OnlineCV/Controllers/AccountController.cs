using AutoMapper;
using CVEntity.Account;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Configuration;
using OnlineCV.UnitOfWork;
using System.Security.Claims;
using OnlineCV.Models;

namespace OnlineCV.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public AccountController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto logindto)
        {
            return View();
        }

        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(CredentialDto credentialdto)
        {
            var registerdto = credentialdto.register;
            // Clear any existing validation errors for the Login property
            ModelState.Remove(nameof(credentialdto.login));
            if (ModelState.IsValid)
            {
                bool emailExists = await _unitOfWork.Credential.EmailExistsAsync(registerdto.Email);
                if (emailExists) {
                    ModelState.AddModelError(string.Empty, "Email already exist.");
                    return View(registerdto);
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
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimprincipal);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return StatusCode(500, "An error occurred while saving the credential");
                    }
                    
                }
            }
            return View(registerdto);
        }
    }
}
