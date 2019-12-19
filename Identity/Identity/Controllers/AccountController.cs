using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager,
                              IUserClaimsPrincipalFactory<User> claimsPrincipalFactory,
                              SignInManager<User> signInManager)
        {
            this._userManager = userManager;
            this._claimsPrincipalFactory = claimsPrincipalFactory;
            this._signInManager = signInManager;
        }

        //
        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await this._userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    user = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = model.UserName,
                        Email = model.Email
                    };

                    var result = await this._userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var token = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationEmail = Url.Action("ConfirmEmailAddress", "Account", new
                        {
                            token,
                            user.Email
                        }, Request.Scheme);

                        System.IO.File.WriteAllText("confirmationLink.txt", confirmationEmail);
                        return View("Success");
                    }
                }
            }

            return View();
        }
        #endregion

        //
        #region Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await this._userManager.FindByNameAsync(model.UserName);

                if (user != null && await this._userManager.CheckPasswordAsync(user, model.Password))
                {
                    if (!await this._userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Email is not confirmed");
                        return View();
                    }

                    var principal = await this._claimsPrincipalFactory.CreateAsync(user);

                    await HttpContext.SignInAsync("Identity.Application", principal);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid UserName or Password");
            }
            return View();
        }
        #endregion

        //
        #region ResetPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await this._userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var token = await this._userManager.GeneratePasswordResetTokenAsync(user);
                    var resetUrl = Url.Action("ResetPassword", "Account", new
                    {
                        token,
                        user.Email
                    }, Request.Scheme);

                    System.IO.File.WriteAllText("resetLink.txt", resetUrl);
                    return View("Success");
                }
                else
                {
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            return View(new ResetPasswordModel
            {
                Token = token,
                Email = email
            });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await this._userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await this._userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View();
                    }
                    return View("Success");
                }
                ModelState.AddModelError("", "Invalid Request!");
            }
            return View();
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailAddress(string token, string email) 
        {
            var user =await this._userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await this._userManager.ConfirmEmailAsync(user,token);
                
                if (result.Succeeded)
                {
                    return View("Success");
                }
            }
            return RedirectToAction("Error","Home");
        }
    }
}
