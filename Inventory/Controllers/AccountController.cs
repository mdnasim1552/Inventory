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
using Microsoft.CodeAnalysis.Scripting;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Inventory.Hubs;
using System.Data;


namespace Inventory.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<UserHub> _userHubContext;
        public AccountController(IMapper mapper, IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment,IConfiguration configuration, IHubContext<UserHub> userhubContext)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _configuration= configuration;
            _userHubContext=userhubContext;
        }
        public IActionResult Signin()
        {
            // Check if the user is already authenticated
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Redirect to the Home page if the user is already authenticated
                return RedirectToAction("Index", "Home");
            }
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
                var userInfo = await _unitOfWork.Credential.SingleOrDefaultAsync(c=>c.Email==logindto.Email);
                //var role = await _unitOfWork.Userrole.SingleOrDefaultAsync(u => u.RoleId == userInfo.RoleId);
                if (userInfo != null && BCrypt.Net.BCrypt.Verify(logindto.Password, userInfo.Password))
                {
                    await ClaimsIdentitySignInAuthentication(userInfo, logindto.IsRemember);                    
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
        private async Task ClaimsIdentitySignInAuthentication(Credential userInfo, bool IsRemember=false)
        {
            var role = await _unitOfWork.Userrole.SingleOrDefaultAsync(u => u.RoleId == userInfo.RoleId);
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,userInfo.Name),
                        new Claim(ClaimTypes.Email,userInfo.Email),
                        new Claim(ClaimTypes.Role,role.Role),
                        new Claim(ClaimTypes.Gender,userInfo.Gender!=null?userInfo.Gender:""),
                        new Claim("UserID",userInfo.Id.ToString()),
                        new Claim("AdminID",userInfo.ParentId !=null?userInfo.ParentId.ToString():userInfo.Id.ToString()),
                        new Claim("Designation",userInfo.Designation!=null?userInfo.Designation:"Designation"),
                        new Claim("Image",String.IsNullOrEmpty( userInfo.Image) ? "/userimages/profile.png":userInfo.Image)
                        //new Claim("Department","HR"),
                        //new Claim("Admin","true")
                     };
            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);//we can use CookieAuthenticationDefaults.AuthenticationScheme (constant) instead of "MyCookieAuth"
            ClaimsPrincipal claimprincipal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = IsRemember
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimprincipal, authProperties);
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
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var userInfo = await _unitOfWork.Credential.SingleOrDefaultAsync(c => c.Email == email);
            if (userInfo == null) return RedirectToAction("ForgotPasswordConfirmation");

            // Generate a secure token
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            //Store the token in the database with an expiration time
            var passwordResetToken = new PasswordResetToken
            {
                Email = email,
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1) // Token valid for 1 hour
            };
            _unitOfWork.PasswordResetToken.Add(passwordResetToken);
            var saveResult=await _unitOfWork.SaveAsync();
            var callbackUrl = Url.Action("ResetPassword", "Account", new { token = token }, protocol: HttpContext.Request.Scheme);
            Credential credentials=await _unitOfWork.Credential.SingleOrDefaultAsync(c=>c.Email==email);
            var adminEmail = _configuration.GetValue<string>("GlobalAdmin:Gmail");
            string filepath = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplate\\Forgotpassword.cshtml");
            string htmlstring = System.IO.File.ReadAllText(filepath);
            htmlstring = htmlstring.Replace("{{company}}", "Inventory Management");
            htmlstring = htmlstring.Replace("{{User's Name}}", credentials.Name);
            htmlstring = htmlstring.Replace("{{Username}}", credentials.Email);
            htmlstring = htmlstring.Replace("{{reseturl}}", callbackUrl);
            bool status = await _unitOfWork.EmailSetting.SendEmailAsync(adminEmail, credentials.Email, "Password Reset for Inventory Management", htmlstring);

            return RedirectToAction("ForgotPasswordConfirmation");
        }
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return View("Error");
            }
            return View(new ResetPasswordDto { Token = token });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var resetToken = await _unitOfWork.PasswordResetToken.SingleOrDefaultAsync(t => t.Token == model.Token && t.Email==model.Email);
            if (resetToken == null || resetToken.Expiration < DateTime.UtcNow)
            {
                // Token is invalid or expired
                ModelState.AddModelError(string.Empty, "Invalid or expired token.");
                return View(model);
            }

            var user = await _unitOfWork.Credential.SingleOrDefaultAsync(c => c.Email == resetToken.Email);
            if (user == null)
            {
                // User not found
                return RedirectToAction("ResetPasswordConfirmation");
            }

            // Hash the password before saving it
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Reset the user's password
            user.Password = hashedPassword;
            //user.IsActive = false;
            _unitOfWork.Credential.Update(user);
            bool userUpdateStatus =await _unitOfWork.SaveAsync();

            // Remove the token after successful reset
            _unitOfWork.PasswordResetToken.Remove(resetToken);
            bool removeToken=await _unitOfWork.SaveAsync();

            return RedirectToAction("ResetPasswordConfirmation");
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
                    var credentials = _mapper.Map<Credential>(registerdto);
                    credentials.Password = BCrypt.Net.BCrypt.HashPassword(registerdto.Password);
                    var role = await _unitOfWork.Userrole.SingleOrDefaultAsync(u => u.Role == "Admin");
                    credentials.RoleId = role.RoleId;
                    credentials.CreatedOn = DateTime.Now;
                    //credentials.IsActive = true;
                    _unitOfWork.Credential.Add(credentials);
                    var saveResult = await _unitOfWork.SaveAsync();
                    if (saveResult)
                    {
                        int generatedId = credentials.Id;

                        string url = _configuration.GetValue<string>("Urls:LoginUrl");
                        var adminEmail = _configuration.GetValue<string>("GlobalAdmin:Gmail");
                        string filepath = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplate\\Welcome.cshtml");
                        string htmlstring = System.IO.File.ReadAllText(filepath);
                        htmlstring = htmlstring.Replace("{{company}}", "Inventory Management");
                        htmlstring = htmlstring.Replace("{{User's Name}}", credentials.Name);
                        htmlstring = htmlstring.Replace("{{Username}}", credentials.Email);
                        htmlstring = htmlstring.Replace("{{Password}}", registerdto.Password);//{{lgnurl}}
                        htmlstring = htmlstring.Replace("{{lgnurl}}", url);
                        bool status = await _unitOfWork.EmailSetting.SendEmailAsync(adminEmail,credentials.Email, "Account Created", htmlstring);


                        var userInfo = await _unitOfWork.Credential.SingleOrDefaultAsync(c => c.Id == generatedId);
                        await ClaimsIdentitySignInAuthentication(userInfo, false);                       
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
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
